using System;
using System.IO;
using System.Linq;

namespace MineServerGUI.Utilities
{
    public class ServerBackupManager
    {
        private readonly string _backupDirectory;

        public ServerBackupManager(string serverDirectory)
        {
            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
            _backupDirectory = Path.Combine(baseDir, "backups", "version-change-backups");
            
            if (!Directory.Exists(_backupDirectory))
            {
                Directory.CreateDirectory(_backupDirectory);
            }
        }

        public string CreateBackup(string serverDirectory, string profileName, string version)
        {
            if (!Directory.Exists(serverDirectory))
            {
                throw new DirectoryNotFoundException($"Server directory not found: {serverDirectory}");
            }

            // Create backup folder with timestamp
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var backupFolderName = $"{profileName}_{version}_{timestamp}";
            var backupPath = Path.Combine(_backupDirectory, backupFolderName);
            
            Directory.CreateDirectory(backupPath);

            try
            {
                // Copy world directories
                var worldDirectories = Directory.GetDirectories(serverDirectory)
                    .Where(d => Path.GetFileName(d).StartsWith("world", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var worldDir in worldDirectories)
                {
                    var worldName = Path.GetFileName(worldDir);
                    var destWorldPath = Path.Combine(backupPath, worldName);
                    CopyDirectory(worldDir, destWorldPath);
                }

                // Copy important files
                var importantFiles = new[] { "server.properties", "whitelist.json", "ops.json", "banned-players.json", "banned-ips.json", "usercache.json" };
                foreach (var fileName in importantFiles)
                {
                    var sourceFile = Path.Combine(serverDirectory, fileName);
                    if (File.Exists(sourceFile))
                    {
                        var destFile = Path.Combine(backupPath, fileName);
                        File.Copy(sourceFile, destFile, true);
                    }
                }

                // Create backup info file
                var infoFile = Path.Combine(backupPath, "backup-info.txt");
                File.WriteAllText(infoFile, 
                    $"Backup created: {DateTime.Now}\n" +
                    $"Profile: {profileName}\n" +
                    $"Version: {version}\n" +
                    $"Server Directory: {serverDirectory}\n" +
                    $"Worlds backed up: {worldDirectories.Count}");

                return backupPath;
            }
            catch (Exception ex)
            {
                // Clean up on error
                if (Directory.Exists(backupPath))
                {
                    try { Directory.Delete(backupPath, true); } catch { }
                }
                throw new Exception($"Failed to create backup: {ex.Message}", ex);
            }
        }

        private void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile, true);
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                var subDirName = Path.GetFileName(subDir);
                var destSubDir = Path.Combine(destDir, subDirName);
                CopyDirectory(subDir, destSubDir);
            }
        }
    }
}

