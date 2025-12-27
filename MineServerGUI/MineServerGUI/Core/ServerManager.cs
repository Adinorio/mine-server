using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MineServerGUI.Models;
using MineServerGUI.Utilities;

namespace MineServerGUI.Core
{
    public class ServerManager
    {
        private Process? _serverProcess;
        private ServerProfile? _currentProfile;
        private string _serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server");
        private string _serverJar = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server", "server.jar");

        public bool IsRunning => _serverProcess != null && !_serverProcess.HasExited;

        public string? JavaPath { get; private set; }
        public string MinMemory { get; set; } = "2G";
        public string MaxMemory { get; set; } = "4G";
        public ServerProfile? CurrentProfile => _currentProfile;

        private readonly JavaManager _javaManager;

        public ServerManager()
        {
            _javaManager = new JavaManager();
            FindJava();
        }

        public void SetProfile(ServerProfile profile)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Cannot change profile while server is running!");
            }

            _currentProfile = profile;
            _serverPath = profile.ServerDirectory;
            _serverJar = profile.ServerJarPath;
            MinMemory = profile.MinMemory;
            MaxMemory = profile.MaxMemory;
        }

        private void FindJava()
        {
            // Use JavaManager to find Java (checks bundled first, then system)
            JavaPath = _javaManager.FindJava();
        }

        /// <summary>
        /// Refreshes Java path (public method for UI to call after Java installation)
        /// </summary>
        public void RefreshJava()
        {
            FindJava();
        }

        /// <summary>
        /// Finds Java for a specific Minecraft version requirement
        /// </summary>
        public string? FindJavaForVersion(string minecraftVersion)
        {
            var requiredJava = VersionHelper.GetRequiredJavaMajorVersion(minecraftVersion);
            return _javaManager.FindJava(requiredJava);
        }

        /// <summary>
        /// Checks if Java is compatible and offers to download if needed
        /// </summary>
        public JavaCheckResult CheckJavaCompatibility(string minecraftVersion)
        {
            var requiredJava = VersionHelper.GetRequiredJavaMajorVersion(minecraftVersion);
            return _javaManager.CheckJava(requiredJava, JavaPath);
        }

        public void StartServer()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Server is already running!");
            }

            if (JavaPath == null)
            {
                throw new FileNotFoundException("Java is not installed or not found in PATH!");
            }

            if (!File.Exists(_serverJar))
            {
                throw new FileNotFoundException($"Server JAR not found: {_serverJar}");
            }

            // Validate Java version compatibility with Minecraft version
            ValidateJavaVersion();

            var serverDir = Path.GetFullPath(_serverPath);
            if (!Directory.Exists(serverDir))
            {
                throw new DirectoryNotFoundException($"Server directory not found: {serverDir}");
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = JavaPath,
                Arguments = $"-Xms{MinMemory} -Xmx{MaxMemory} -jar server.jar nogui",
                WorkingDirectory = serverDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = false
            };

            _serverProcess = Process.Start(startInfo);
            
            if (_serverProcess == null)
            {
                throw new Exception("Failed to start server process!");
            }
        }

        /// <summary>
        /// Validates that the current Java version is compatible with the Minecraft server version
        /// </summary>
        private void ValidateJavaVersion()
        {
            if (JavaPath == null)
                return; // Already handled in StartServer

            // Detect Minecraft version from server.jar
            var mcVersion = ServerVersionDetector.DetectVersion(_serverJar, JavaPath);
            if (string.IsNullOrEmpty(mcVersion))
            {
                // Can't detect version, skip validation
                System.Diagnostics.Debug.WriteLine("[ServerManager] Could not detect MC version, skipping Java validation");
                return;
            }

            // Get required Java version for this MC version
            var requiredJava = VersionHelper.GetRequiredJavaVersion(mcVersion);
            var requiredJavaMajor = VersionHelper.GetRequiredJavaMajorVersion(mcVersion);

            // Get current Java version
            var currentJavaMajor = JavaVersionChecker.GetJavaMajorVersion(JavaPath);
            
            if (currentJavaMajor == 0)
            {
                // Could not detect Java version, show warning but continue
                System.Diagnostics.Debug.WriteLine($"[ServerManager] Could not detect Java version, but JavaPath exists: {JavaPath}");
                return;
            }

            // Check compatibility
            if (currentJavaMajor < requiredJavaMajor)
            {
                var downloadUrl = requiredJavaMajor == 21 
                    ? "https://adoptium.net/temurin/releases/?version=21"
                    : requiredJavaMajor == 17
                    ? "https://adoptium.net/temurin/releases/?version=17"
                    : "https://adoptium.net/temurin/releases/";

                throw new InvalidOperationException(
                    $"Java version mismatch!\n\n" +
                    $"Minecraft {mcVersion} requires {requiredJava}\n" +
                    $"Current Java version: {currentJavaMajor}\n\n" +
                    $"Please install {requiredJava}:\n" +
                    $"1. Go to: {downloadUrl}\n" +
                    $"2. Download JDK {requiredJavaMajor} for Windows x64\n" +
                    $"3. Install it (you can keep other Java versions)\n" +
                    $"4. Restart the application\n\n" +
                    $"Note: Multiple Java versions can coexist on your system."
                );
            }

            System.Diagnostics.Debug.WriteLine($"[ServerManager] Java version check passed: MC {mcVersion} requires {requiredJava}, found Java {currentJavaMajor}");
        }

        public void StopServer()
        {
            if (!IsRunning)
            {
                return;
            }

            try
            {
                // Send "stop" command to server
                if (_serverProcess != null && !_serverProcess.HasExited)
                {
                    _serverProcess.StandardInput.WriteLine("stop");
                    _serverProcess.StandardInput.Flush();
                    
                    // Wait for graceful shutdown (max 10 seconds)
                    if (!_serverProcess.WaitForExit(10000))
                    {
                        // Force kill if still running
                        _serverProcess.Kill();
                        _serverProcess.WaitForExit();
                    }
                }
            }
            catch
            {
                // If we can't send stop command, force kill
                try
                {
                    _serverProcess?.Kill();
                }
                catch { }
            }
            finally
            {
                _serverProcess?.Dispose();
                _serverProcess = null;
            }
        }

        public void RestartServer()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Server is not running!");
            }

            StopServer();
            
            // Wait for server to fully stop (max 10 seconds)
            int waitTime = 0;
            while (_serverProcess != null && !_serverProcess.HasExited && waitTime < 10000)
            {
                System.Threading.Thread.Sleep(500);
                waitTime += 500;
            }
            
            // Additional wait to ensure clean shutdown
            System.Threading.Thread.Sleep(2000);
            
            StartServer();
        }
    }
}

