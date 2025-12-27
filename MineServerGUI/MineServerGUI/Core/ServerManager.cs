using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MineServerGUI.Models;

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

        public ServerManager()
        {
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
                {
                    JavaPath = location;
                    return;
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
                JavaPath = "java";
            }
            catch
            {
                JavaPath = null;
            }
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

