using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace MineServerGUI.Core
{
    public static class ServerVersionDetector
    {
        /// <summary>
        /// Detects the Minecraft version from a server.jar file
        /// </summary>
        public static string? DetectVersion(string serverJarPath, string? javaPath = null)
        {
            if (!File.Exists(serverJarPath))
                return null;

            try
            {
                // Method 1: Try to extract from JAR manifest
                var versionFromManifest = TryGetVersionFromManifest(serverJarPath);
                if (!string.IsNullOrEmpty(versionFromManifest))
                    return versionFromManifest;

                // Method 2: Try to find Java if not provided
                if (string.IsNullOrEmpty(javaPath))
                {
                    javaPath = FindJava();
                    if (string.IsNullOrEmpty(javaPath))
                        return null;
                }

                // Method 3: Try running server with --version flag
                var versionFromCommand = TryGetVersionFromCommand(javaPath, serverJarPath);
                if (!string.IsNullOrEmpty(versionFromCommand))
                    return versionFromCommand;

                // Method 4: Try running server briefly to get startup message
                var versionFromStartup = TryGetVersionFromStartup(javaPath, serverJarPath);
                if (!string.IsNullOrEmpty(versionFromStartup))
                    return versionFromStartup;
            }
            catch (Exception ex)
            {
                // Log error for debugging (could be improved with proper logging)
                System.Diagnostics.Debug.WriteLine($"Version detection error: {ex.Message}");
            }

            return null;
        }

        private static string? TryGetVersionFromManifest(string jarPath)
        {
            try
            {
                // Method 1: Check filename for version pattern (e.g., server-1.21.11.jar)
                var fileName = Path.GetFileNameWithoutExtension(jarPath);
                var fileNameMatch = Regex.Match(fileName, @"(\d+\.\d+(?:\.\d+)?)", RegexOptions.IgnoreCase);
                if (fileNameMatch.Success)
                {
                    var version = fileNameMatch.Groups[1].Value;
                    // Validate it's a Minecraft version, not Java version
                    if (IsValidMinecraftVersion(version))
                        return version;
                }

                // Method 2: Try to read version from JAR manifest
                using var archive = ZipFile.OpenRead(jarPath);
                
                // Check manifest
                var manifestEntry = archive.GetEntry("META-INF/MANIFEST.MF");
                if (manifestEntry != null)
                {
                    using var reader = new StreamReader(manifestEntry.Open());
                    var content = reader.ReadToEnd();
                    
                    // Look for version in manifest (various formats)
                    // BUT exclude Java versions (1.0, 1.8, etc.)
                    var patterns = new[]
                    {
                        @"Implementation-Version:\s*(\d+\.\d+(?:\.\d+)?)",
                        @"Specification-Version:\s*(\d+\.\d+(?:\.\d+)?)",
                        @"Version:\s*(\d+\.\d+(?:\.\d+)?)"
                    };
                    
                    foreach (var pattern in patterns)
                    {
                        var versionMatch = Regex.Match(content, pattern, RegexOptions.IgnoreCase);
                        if (versionMatch.Success)
                        {
                            var version = versionMatch.Groups[1].Value;
                            // Validate it's a Minecraft version, not Java version
                            if (IsValidMinecraftVersion(version))
                                return version;
                        }
                    }
                }

                // Method 3: Try to find version.json inside JAR (Minecraft sometimes includes this)
                var versionJsonEntry = archive.GetEntry("version.json");
                if (versionJsonEntry != null)
                {
                    using var reader = new StreamReader(versionJsonEntry.Open());
                    var jsonContent = reader.ReadToEnd();
                    var versionMatch = Regex.Match(jsonContent, @"""id""\s*:\s*""(\d+\.\d+(?:\.\d+)?)""", RegexOptions.IgnoreCase);
                    if (versionMatch.Success)
                    {
                        var version = versionMatch.Groups[1].Value;
                        if (IsValidMinecraftVersion(version))
                            return version;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Manifest detection error: {ex.Message}");
            }
            return null;
        }

        private static string? TryGetVersionFromCommand(string javaPath, string jarPath)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = javaPath,
                    Arguments = $"-jar \"{jarPath}\" --version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(jarPath)
                };

                using var process = Process.Start(startInfo);
                if (process == null)
                    return null;

                // Wait for process with shorter timeout to avoid hanging
                if (!process.WaitForExit(2000)) // Reduced to 2 seconds
                {
                    try { process.Kill(); } catch { }
                    return null;
                }

                // Read outputs with timeout
                string stderr = "";
                string stdout = "";
                
                try
                {
                    var readStderr = Task.Run(() => stderr = process.StandardError.ReadToEnd());
                    var readStdout = Task.Run(() => stdout = process.StandardOutput.ReadToEnd());
                    
                    // Wait max 1 second for reading to complete
                    Task.WaitAll(new[] { readStderr, readStdout }, 1000);
                }
                catch
                {
                    // Reading timed out or failed, continue with what we have
                }

                // Try to parse version from output (multiple patterns)
                // IMPORTANT: Exclude Java version output (e.g., "java version 1.8.0")
                var output = stderr + stdout;
                
                // Remove Java version lines to avoid false matches
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var filteredOutput = string.Join("\n", lines.Where(line => 
                    !line.Contains("java version", StringComparison.OrdinalIgnoreCase) &&
                    !line.Contains("openjdk version", StringComparison.OrdinalIgnoreCase) &&
                    !line.Contains("java.lang", StringComparison.OrdinalIgnoreCase) &&
                    !line.StartsWith("Picked up", StringComparison.OrdinalIgnoreCase)
                ));
                
                var patterns = new[]
                {
                    @"Starting\s+minecraft\s+server\s+version\s+(\d+\.\d+(?:\.\d+)?)",
                    @"Minecraft\s+server\s+version\s+(\d+\.\d+(?:\.\d+)?)",
                    @"minecraft.*version\s+(\d+\.\d+(?:\.\d+)?)",
                    @"server\s+version\s+(\d+\.\d+(?:\.\d+)?)",
                };
                
                    foreach (var pattern in patterns)
                    {
                        var versionMatch = Regex.Match(filteredOutput, pattern, RegexOptions.IgnoreCase);
                        if (versionMatch.Success)
                        {
                            var version = versionMatch.Groups[1].Value;
                            // Validate it's a Minecraft version, not Java version
                            if (IsValidMinecraftVersion(version))
                                return version;
                        }
                    }
            }
            catch
            {
                // Command execution failed
            }
            return null;
        }

        private static string? TryGetVersionFromStartup(string javaPath, string jarPath)
        {
            // Skip this method - it's too slow and can hang
            // The manifest and command methods should be sufficient
            return null;
        }

        private static bool IsValidMinecraftVersion(string version)
        {
            if (string.IsNullOrEmpty(version) || !version.StartsWith("1."))
                return false;

            // Reject common Java versions
            var javaVersions = new[] { "1.0", "1.1", "1.2", "1.3", "1.4", "1.5", "1.6", "1.7", "1.8", "1.9" };
            if (javaVersions.Contains(version))
                return false;

            var parts = version.Split('.');
            if (parts.Length < 2)
                return false;

            // Minecraft versions are typically:
            // - 1.7.x to 1.20.x (older versions)
            // - 1.21.x, 1.22.x, etc. (newer versions)
            // - Usually have 3 parts (1.x.x) for modern versions
            if (int.TryParse(parts[1], out var minor))
            {
                // Accept if minor version is 7+ (Minecraft 1.7+)
                // OR if it has 3 parts (like 1.21.11)
                if (minor >= 7 || parts.Length >= 3)
                    return true;
            }

            return false;
        }

        private static string? FindJava()
        {
            // Try common Java locations
            string[] javaLocations = {
                @"C:\Program Files\Java\jdk-25\bin\java.exe",
                @"C:\Program Files\Java\jdk-21\bin\java.exe",
                @"C:\Program Files\Eclipse Adoptium\jdk-25-hotspot\bin\java.exe",
                @"C:\Program Files\Eclipse Adoptium\jdk-21-hotspot\bin\java.exe"
            };

            foreach (var location in javaLocations)
            {
                if (File.Exists(location))
                    return location;
            }

            // Try PATH
            try
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "java",
                    Arguments = "-version",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                });
                process?.WaitForExit();
                return "java";
            }
            catch
            {
                return null;
            }
        }
    }
}

