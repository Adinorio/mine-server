using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MineServerGUI.Utilities;

namespace MineServerGUI.Core
{
    /// <summary>
    /// Manages Java installation, detection, and automatic download
    /// </summary>
    public class JavaManager
    {
        private readonly string _javaInstallPath;
        // Adoptium API supports Java 8, 11, 16, 17, 21, and newer versions
        private const string AdoptiumApiUrl = "https://api.adoptium.net/v3/assets/latest/{0}/hotspot?architecture=x64&image_type=jdk&os=windows";
        
        /// <summary>
        /// Checks if a Java version is supported for download
        /// </summary>
        public static bool IsJavaVersionSupported(int version)
        {
            // Adoptium supports: 8, 11, 16, 17, 21, and newer LTS versions
            return version == 8 || version == 11 || version == 16 || version == 17 || version == 21 || version >= 21;
        }
        
        public JavaManager()
        {
            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
            _javaInstallPath = Path.Combine(baseDir, "java-runtimes");
            
            if (!Directory.Exists(_javaInstallPath))
            {
                Directory.CreateDirectory(_javaInstallPath);
            }
        }

        /// <summary>
        /// Finds Java installation, checking bundled runtimes first, then system installations
        /// </summary>
        public string? FindJava(int? requiredVersion = null)
        {
            // First, check bundled Java runtimes (in our app directory)
            if (requiredVersion.HasValue)
            {
                var bundledJava = FindBundledJava(requiredVersion.Value);
                if (!string.IsNullOrEmpty(bundledJava))
                {
                    System.Diagnostics.Debug.WriteLine($"[JavaManager] Found bundled Java {requiredVersion.Value} at: {bundledJava}");
                    return bundledJava;
                }
            }

            // Check all bundled versions
            var allBundled = FindAllBundledJava();
            if (allBundled.Count > 0)
            {
                // If specific version required, try to find it
                if (requiredVersion.HasValue && allBundled.ContainsKey(requiredVersion.Value))
                {
                    return allBundled[requiredVersion.Value];
                }
                // Otherwise, use the highest version
                var highestVersion = 0;
                string? highestPath = null;
                foreach (var kvp in allBundled)
                {
                    if (kvp.Key > highestVersion)
                    {
                        highestVersion = kvp.Key;
                        highestPath = kvp.Value;
                    }
                }
                if (highestPath != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[JavaManager] Using bundled Java {highestVersion} at: {highestPath}");
                    return highestPath;
                }
            }

            // Fallback to system Java installations
            return FindSystemJava(requiredVersion);
        }

        /// <summary>
        /// Finds bundled Java runtime in our application directory
        /// </summary>
        private string? FindBundledJava(int version)
        {
            var versionDir = Path.Combine(_javaInstallPath, $"jdk-{version}");
            var javaExe = Path.Combine(versionDir, "bin", "java.exe");
            
            if (File.Exists(javaExe))
            {
                return javaExe;
            }

            return null;
        }

        /// <summary>
        /// Finds all bundled Java runtimes
        /// </summary>
        private System.Collections.Generic.Dictionary<int, string> FindAllBundledJava()
        {
            var result = new System.Collections.Generic.Dictionary<int, string>();
            
            if (!Directory.Exists(_javaInstallPath))
                return result;

            foreach (var dir in Directory.GetDirectories(_javaInstallPath))
            {
                var dirName = Path.GetFileName(dir);
                if (dirName.StartsWith("jdk-") && int.TryParse(dirName.Substring(4), out int version))
                {
                    var javaExe = Path.Combine(dir, "bin", "java.exe");
                    if (File.Exists(javaExe))
                    {
                        result[version] = javaExe;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Finds system Java installations
        /// </summary>
        private string? FindSystemJava(int? requiredVersion = null)
        {
            // Try common Java locations
            string[] javaLocations = {
                @"C:\Program Files\Java\jdk-25\bin\java.exe",
                @"C:\Program Files\Java\jdk-21\bin\java.exe",
                @"C:\Program Files\Java\jdk-17\bin\java.exe",
                @"C:\Program Files\Java\jdk-16\bin\java.exe",
                @"C:\Program Files\Java\jdk-8\bin\java.exe",
                @"C:\Program Files\Eclipse Adoptium\jdk-25-hotspot\bin\java.exe",
                @"C:\Program Files\Eclipse Adoptium\jdk-21-hotspot\bin\java.exe",
                @"C:\Program Files\Eclipse Adoptium\jdk-17-hotspot\bin\java.exe",
                @"C:\Program Files\Eclipse Adoptium\jdk-16-hotspot\bin\java.exe",
                @"C:\Program Files\Eclipse Adoptium\jdk-8-hotspot\bin\java.exe"
            };

            foreach (var location in javaLocations)
            {
                if (File.Exists(location))
                {
                    // If specific version required, check it
                    if (requiredVersion.HasValue)
                    {
                        var version = JavaVersionChecker.GetJavaMajorVersion(location);
                        if (version == requiredVersion.Value)
                        {
                            return location;
                        }
                    }
                    else
                    {
                        return location; // Return first found
                    }
                }
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
                
                if (requiredVersion.HasValue)
                {
                    var version = JavaVersionChecker.GetJavaMajorVersion("java");
                    if (version == requiredVersion.Value)
                    {
                        return "java";
                    }
                }
                else
                {
                    return "java";
                }
            }
            catch
            {
                return null;
            }

            return null; // Fallback if nothing found
        }

        /// <summary>
        /// Checks if Java is installed and compatible
        /// </summary>
        public JavaCheckResult CheckJava(int requiredVersion, string? currentJavaPath = null)
        {
            var result = new JavaCheckResult
            {
                RequiredVersion = requiredVersion
            };

            // Try to find Java
            var javaPath = currentJavaPath ?? FindJava(requiredVersion);
            
            if (string.IsNullOrEmpty(javaPath))
            {
                result.IsInstalled = false;
                result.Message = $"Java {requiredVersion} is not installed";
                return result;
            }

            result.JavaPath = javaPath;
            result.IsInstalled = true;

            // Check version
            var currentVersion = JavaVersionChecker.GetJavaMajorVersion(javaPath);
            if (currentVersion == 0)
            {
                result.IsCompatible = false;
                result.Message = "Could not detect Java version";
                return result;
            }

            result.CurrentVersion = currentVersion;
            result.IsCompatible = currentVersion >= requiredVersion;

            if (!result.IsCompatible)
            {
                result.Message = $"Java {currentVersion} is installed, but Java {requiredVersion} is required";
            }
            else
            {
                result.Message = $"Java {currentVersion} is compatible";
            }

            return result;
        }

        /// <summary>
        /// Downloads and installs Java automatically
        /// </summary>
        public async Task<bool> DownloadAndInstallJavaAsync(int version, IProgress<DownloadProgress>? progress = null)
        {
            try
            {
                // Validate version is supported for download
                if (!IsJavaVersionSupported(version))
                {
                    throw new Exception($"Java {version} is not supported for automatic download. " +
                        $"Supported versions: 8, 11, 16, 17, 21, and newer. " +
                        $"Please install Java {version} manually from https://adoptium.net/");
                }

                progress?.Report(new DownloadProgress { Status = $"Downloading Java {version}...", Progress = 0 });

                // Get download URL from Adoptium API
                var downloadUrl = await GetJavaDownloadUrlAsync(version);
                if (string.IsNullOrEmpty(downloadUrl))
                {
                    throw new Exception($"Could not find Java {version} download URL from Adoptium API. " +
                        $"Please install Java {version} manually from https://adoptium.net/");
                }

                progress?.Report(new DownloadProgress { Status = $"Downloading Java {version}...", Progress = 10 });

                // Download Java installer
                var installerPath = Path.Combine(_javaInstallPath, $"jdk-{version}-installer.zip");
                await DownloadFileAsync(downloadUrl, installerPath, progress);

                progress?.Report(new DownloadProgress { Status = $"Extracting Java {version}...", Progress = 80 });

                // Extract Java
                var extractPath = Path.Combine(_javaInstallPath, $"jdk-{version}");
                System.IO.Compression.ZipFile.ExtractToDirectory(installerPath, extractPath, true);

                // Find java.exe in extracted files
                var javaExe = FindJavaExeInDirectory(extractPath);
                if (string.IsNullOrEmpty(javaExe))
                {
                    throw new Exception("Could not find java.exe in downloaded package");
                }

                // Clean up installer
                try
                {
                    File.Delete(installerPath);
                }
                catch { }

                progress?.Report(new DownloadProgress { Status = $"Java {version} installed successfully!", Progress = 100 });

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[JavaManager] Download failed: {ex.Message}");
                throw;
            }
        }

        private async Task<string?> GetJavaDownloadUrlAsync(int version)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                var apiUrl = string.Format(AdoptiumApiUrl, version);
                var response = await httpClient.GetStringAsync(apiUrl);
                
                // Parse JSON response
                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;
                
                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var first = root[0];
                    if (first.TryGetProperty("binary", out var binary))
                    {
                        if (binary.TryGetProperty("package", out var package))
                        {
                            if (package.TryGetProperty("link", out var link))
                            {
                                return link.GetString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[JavaManager] API call failed: {ex.Message}");
            }

            return null;
        }

        private async Task DownloadFileAsync(string url, string destination, IProgress<DownloadProgress>? progress)
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(10); // Java downloads can be large

            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? 0;
            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None);

            var buffer = new byte[8192];
            long totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;

                if (totalBytes > 0 && progress != null)
                {
                    var percent = 10 + (int)((totalBytesRead * 70) / totalBytes); // 10-80% for download
                    progress.Report(new DownloadProgress 
                    { 
                        Status = $"Downloading Java... ({totalBytesRead / 1024 / 1024} MB / {totalBytes / 1024 / 1024} MB)",
                        Progress = percent 
                    });
                }
            }
        }

        private string? FindJavaExeInDirectory(string directory)
        {
            var javaExe = Path.Combine(directory, "bin", "java.exe");
            if (File.Exists(javaExe))
                return javaExe;

            // Sometimes Java is in a subdirectory
            foreach (var subDir in Directory.GetDirectories(directory))
            {
                javaExe = Path.Combine(subDir, "bin", "java.exe");
                if (File.Exists(javaExe))
                    return javaExe;
            }

            return null;
        }
    }

    public class JavaCheckResult
    {
        public bool IsInstalled { get; set; }
        public bool IsCompatible { get; set; }
        public int RequiredVersion { get; set; }
        public int CurrentVersion { get; set; }
        public string? JavaPath { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class DownloadProgress
    {
        public string Status { get; set; } = string.Empty;
        public int Progress { get; set; }
    }
}

