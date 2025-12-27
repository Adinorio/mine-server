using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MineServerGUI.Core
{
    public class ServerDownloader
    {
        private readonly string _serverPath;
        private readonly string _serverJar;
        private readonly string _cachePath;
        private const string VersionManifestUrl = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
        private const string DefaultVersion = "1.21.11"; // TLauncher compatible version

        public event EventHandler<DownloadProgressEventArgs>? DownloadProgress;

        public ServerDownloader()
        {
            _serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server");
            _serverJar = Path.Combine(_serverPath, "server.jar");
            _cachePath = Path.Combine(_serverPath, "cache");
            
            // Ensure cache directory exists
            if (!Directory.Exists(_cachePath))
            {
                Directory.CreateDirectory(_cachePath);
            }
        }

        public bool ServerJarExists()
        {
            return File.Exists(_serverJar);
        }

        /// <summary>
        /// Gets the cache path for a specific version
        /// </summary>
        private string GetCachePathForVersion(string version)
        {
            // Normalize version string (trim, remove any invalid path characters)
            var normalizedVersion = version?.Trim() ?? string.Empty;
            // Remove any invalid path characters just in case
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
            {
                normalizedVersion = normalizedVersion.Replace(c, '_');
            }
            
            var versionCacheDir = Path.Combine(_cachePath, normalizedVersion);
            return Path.Combine(versionCacheDir, "server.jar");
        }

        /// <summary>
        /// Checks if a version exists in the cache
        /// </summary>
        public bool IsVersionCached(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                System.Diagnostics.Debug.WriteLine($"[ServerDownloader] IsVersionCached: Version is null or whitespace");
                return false;
            }

            // Normalize version before checking (trim whitespace)
            var normalizedVersion = version.Trim();
            var cachedPath = GetCachePathForVersion(normalizedVersion);
            var exists = File.Exists(cachedPath);
            
            // Additional validation: check file size (should be > 0)
            if (exists)
            {
                try
                {
                    var fileInfo = new FileInfo(cachedPath);
                    if (fileInfo.Length == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ServerDownloader] IsVersionCached: Cache file exists but is empty (0 bytes), treating as not cached");
                        return false;
                    }
                    System.Diagnostics.Debug.WriteLine($"[ServerDownloader] IsVersionCached('{version}') -> normalized: '{normalizedVersion}', path: {cachedPath}, exists: true, size: {fileInfo.Length} bytes");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ServerDownloader] IsVersionCached: Error checking cache file: {ex.Message}");
                    return false;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[ServerDownloader] IsVersionCached('{version}') -> normalized: '{normalizedVersion}', path: {cachedPath}, exists: false");
            }
            
            return exists;
        }

        /// <summary>
        /// Copies a cached version to the target location
        /// </summary>
        private bool CopyFromCache(string version, string targetPath)
        {
            try
            {
                // Normalize version before looking up cache path
                var normalizedVersion = version?.Trim() ?? string.Empty;
                var cachedPath = GetCachePathForVersion(normalizedVersion);
                
                if (!File.Exists(cachedPath))
                {
                    System.Diagnostics.Debug.WriteLine($"[ServerDownloader] CopyFromCache: Cache file not found at: {cachedPath}");
                    return false;
                }

                // Verify cache file is valid (not empty)
                var cacheFileInfo = new FileInfo(cachedPath);
                if (cacheFileInfo.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[ServerDownloader] CopyFromCache: Cache file is empty (0 bytes) at: {cachedPath}");
                    return false;
                }

                // Ensure target directory exists
                var targetDir = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // Copy with overwrite
                File.Copy(cachedPath, targetPath, overwrite: true);
                
                // Verify copy succeeded
                if (!File.Exists(targetPath))
                {
                    System.Diagnostics.Debug.WriteLine($"[ServerDownloader] CopyFromCache: Copy completed but target file not found: {targetPath}");
                    return false;
                }
                
                var targetFileInfo = new FileInfo(targetPath);
                if (targetFileInfo.Length != cacheFileInfo.Length)
                {
                    System.Diagnostics.Debug.WriteLine($"[ServerDownloader] CopyFromCache: File size mismatch - cache: {cacheFileInfo.Length}, target: {targetFileInfo.Length}");
                    return false;
                }
                
                System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ✓ Successfully copied cached version '{normalizedVersion}' ({cacheFileInfo.Length} bytes) from {cachedPath} to {targetPath}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ServerDownloader] CopyFromCache failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Saves a downloaded file to the cache for future reuse
        /// </summary>
        private void SaveToCache(string version, string sourcePath)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    System.Diagnostics.Debug.WriteLine($"[ServerDownloader] SaveToCache: Source file does not exist: {sourcePath}");
                    return;
                }

                // Validate source file is not empty
                var sourceFileInfo = new FileInfo(sourcePath);
                if (sourceFileInfo.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[ServerDownloader] SaveToCache: Source file is empty (0 bytes), not saving to cache: {sourcePath}");
                    return;
                }

                // Normalize version before saving (trim whitespace)
                var normalizedVersion = version?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(normalizedVersion))
                {
                    System.Diagnostics.Debug.WriteLine($"[ServerDownloader] SaveToCache: Normalized version is empty, not saving to cache");
                    return;
                }

                var cachedPath = GetCachePathForVersion(normalizedVersion);
                var cacheDir = Path.GetDirectoryName(cachedPath);
                
                if (!string.IsNullOrEmpty(cacheDir))
                {
                    if (!Directory.Exists(cacheDir))
                    {
                        Directory.CreateDirectory(cacheDir);
                        System.Diagnostics.Debug.WriteLine($"[ServerDownloader] Created cache directory: {cacheDir}");
                    }

                    // Copy to cache
                    File.Copy(sourcePath, cachedPath, overwrite: true);
                    
                    // Verify copy succeeded
                    if (File.Exists(cachedPath))
                    {
                        var cachedFileInfo = new FileInfo(cachedPath);
                        if (cachedFileInfo.Length == sourceFileInfo.Length && cachedFileInfo.Length > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ✓ Successfully saved version '{normalizedVersion}' ({cachedFileInfo.Length} bytes) to cache: {cachedPath}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ⚠ Cache save completed but file size mismatch - source: {sourceFileInfo.Length}, cache: {cachedFileInfo.Length}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ⚠ Cache save completed but file not found at: {cachedPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail - cache save is not critical
                System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ⚠ Cache save failed (non-critical): {ex.Message}");
            }
        }

        public async Task<List<string>> GetAvailableVersionsAsync()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                
                var manifestJson = await httpClient.GetStringAsync(VersionManifestUrl);
                var manifest = JsonSerializer.Deserialize<VersionManifest>(manifestJson);

                if (manifest == null || manifest.versions == null)
                    return new List<string> { DefaultVersion };

                // Filter to only stable releases (no snapshots, pre-releases, or release candidates)
                // Only include versions that match pattern: x.y.z (like 1.21.11, 1.20.1, etc.)
                var stableVersions = manifest.versions
                    .Where(v => v.id != null && 
                           !v.id.Contains("-") && // Exclude anything with dashes (rc, pre, snapshot)
                           !v.id.Contains("snapshot", StringComparison.OrdinalIgnoreCase) &&
                           !v.id.Contains("rc", StringComparison.OrdinalIgnoreCase) &&
                           !v.id.Contains("pre", StringComparison.OrdinalIgnoreCase) &&
                           !v.id.Contains("w", StringComparison.OrdinalIgnoreCase) && // Snapshots like 25w46a
                           System.Text.RegularExpressions.Regex.IsMatch(v.id!, @"^\d+\.\d+(\.\d+)?$")) // Only x.y.z format
                    .Select(v => v.id!)
                    .Distinct() // Remove duplicates
                    .ToList(); // Show ALL stable versions, no limit

                return stableVersions.Count > 0 ? stableVersions : new List<string> { DefaultVersion };
            }
            catch
            {
                return new List<string> { DefaultVersion };
            }
        }

        public async Task<bool> DownloadServerJarAsync(string version, bool overwrite = false)
        {
            try
            {
                // Validate and normalize version parameter (trim whitespace for consistent cache lookup)
                if (string.IsNullOrWhiteSpace(version))
                {
                    throw new ArgumentException("Version cannot be empty", nameof(version));
                }
                
                // Normalize version string - trim whitespace to ensure cache matching works
                version = version.Trim();
                
                System.Diagnostics.Debug.WriteLine($"[ServerDownloader] DownloadServerJarAsync called with version='{version}', overwrite={overwrite}");

                // Ensure server directory exists
                if (!Directory.Exists(_serverPath))
                {
                    Directory.CreateDirectory(_serverPath);
                }

                // Check cache first - if version exists in cache, copy from cache instead of downloading
                // IMPORTANT: Only use cache for the EXACT version requested, not any other version
                // Cache check happens BEFORE overwrite logic - cache should always be used if available
                // This prevents unnecessary downloads when the file is already cached
                var cacheCheckResult = IsVersionCached(version);
                if (cacheCheckResult)
                {
                    System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ✓ Version '{version}' found in cache, using cached copy");
                    OnDownloadProgress(new DownloadProgressEventArgs { Status = $"Version {version} found in cache, copying...", Progress = 10 });
                    
                    // Always delete existing file if overwriting
                    if (overwrite && ServerJarExists())
                    {
                        try
                        {
                            File.Delete(_serverJar);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to delete existing server.jar: {ex.Message}", ex);
                        }
                    }

                    // Copy from cache - CopyFromCache will normalize the version internally and validate
                    var copyResult = CopyFromCache(version, _serverJar);
                    if (copyResult)
                    {
                        // CopyFromCache already validates the file, but double-check
                        if (File.Exists(_serverJar))
                        {
                            var fileInfo = new FileInfo(_serverJar);
                            if (fileInfo.Length > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ✓ Successfully used cached version '{version}' ({fileInfo.Length} bytes) - NO DOWNLOAD NEEDED");
                                OnDownloadProgress(new DownloadProgressEventArgs { Status = $"✓ Using cached version {version} (no download needed)", Progress = 100 });
                                return true; // ✅ SUCCESS - Return early, don't proceed to download
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ⚠ Copied file is empty (0 bytes), will download");
                                OnDownloadProgress(new DownloadProgressEventArgs { Status = "Cache file is corrupted, downloading instead...", Progress = 0 });
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ⚠ CopyFromCache returned true but target file not found, will download");
                            OnDownloadProgress(new DownloadProgressEventArgs { Status = "Cache copy failed, downloading instead...", Progress = 0 });
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ⚠ CopyFromCache returned false, will download");
                        OnDownloadProgress(new DownloadProgressEventArgs { Status = "Cache copy failed, downloading instead...", Progress = 0 });
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ✗ Version '{version}' NOT found in cache, will download");
                }

                // Always delete existing file if overwriting
                if (overwrite && ServerJarExists())
                {
                    OnDownloadProgress(new DownloadProgressEventArgs { Status = "Removing existing server.jar...", Progress = 0 });
                    try
                    {
                        File.Delete(_serverJar);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to delete existing server.jar: {ex.Message}", ex);
                    }
                }
                // Check if already exists (only skip if not overwriting)
                else if (!overwrite && ServerJarExists())
                {
                    OnDownloadProgress(new DownloadProgressEventArgs { Status = "Server JAR already exists, skipping download.", Progress = 100 });
                    return true;
                }

                OnDownloadProgress(new DownloadProgressEventArgs { Status = $"Fetching version information for {version}...", Progress = 0 });

                // Get version manifest
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(5);
                
                var manifestJson = await httpClient.GetStringAsync(VersionManifestUrl);
                var manifest = JsonSerializer.Deserialize<VersionManifest>(manifestJson);

                if (manifest == null)
                {
                    throw new Exception("Failed to parse version manifest");
                }

                // Find version (exact match, case-sensitive)
                var versionInfo = Array.Find(manifest.versions, v => v.id != null && v.id.Equals(version, StringComparison.Ordinal));
                if (versionInfo == null)
                {
                    // Try case-insensitive match as fallback
                    versionInfo = Array.Find(manifest.versions, v => v.id != null && v.id.Equals(version, StringComparison.OrdinalIgnoreCase));
                    if (versionInfo == null)
                    {
                        // Debug: Show available versions near the requested one
                        var similarVersions = manifest.versions
                            .Where(v => v.id != null && v.id.Contains(version.Split('.')[0] + "." + version.Split('.')[1]))
                            .Take(5)
                            .Select(v => v.id)
                            .ToList();
                        
                        var errorMsg = $"Version '{version}' not found in manifest.";
                        if (similarVersions.Count > 0)
                        {
                            errorMsg += $"\n\nSimilar versions found: {string.Join(", ", similarVersions)}";
                        }
                        throw new Exception(errorMsg);
                    }
                }

                OnDownloadProgress(new DownloadProgressEventArgs { Status = $"Found version {version}, fetching details...", Progress = 10 });

                // Get version details
                var versionDetailsJson = await httpClient.GetStringAsync(versionInfo.url);
                var versionDetails = JsonSerializer.Deserialize<VersionDetails>(versionDetailsJson);

                if (versionDetails == null || versionDetails.downloads?.server == null)
                {
                    throw new Exception("Failed to get server download URL");
                }

                var downloadUrl = versionDetails.downloads.server.url;
                var fileSize = versionDetails.downloads.server.size;

                OnDownloadProgress(new DownloadProgressEventArgs { Status = $"Downloading server.jar ({fileSize / 1024 / 1024} MB)...", Progress = 20 });

                // Download server.jar
                using var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(_serverJar, FileMode.Create, FileAccess.Write, FileShare.None);

                var totalBytes = response.Content.Headers.ContentLength ?? fileSize;
                var buffer = new byte[8192];
                long totalBytesRead = 0;
                int bytesRead;

                int lastReportedProgress = 0;
                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;

                    var progress = 20 + (int)((totalBytesRead * 80) / totalBytes);
                    
                    // Only report progress every 1% change to reduce event spam and ensure UI thread can process
                    if (progress != lastReportedProgress || totalBytesRead == totalBytes)
                    {
                        lastReportedProgress = progress;
                        OnDownloadProgress(new DownloadProgressEventArgs 
                        { 
                            Status = $"Downloading... ({totalBytesRead / 1024 / 1024} MB / {totalBytes / 1024 / 1024} MB)",
                            Progress = progress 
                        });
                        
                        // Yield to allow UI thread to process messages
                        await Task.Yield();
                    }
                }

                OnDownloadProgress(new DownloadProgressEventArgs { Status = "Download complete!", Progress = 100 });
                
                // Save downloaded file to cache for future reuse
                // This MUST happen after download completes successfully
                try
                {
                    SaveToCache(version, _serverJar);
                    var cachePath = GetCachePathForVersion(version);
                    if (File.Exists(cachePath))
                    {
                        System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ✓ Successfully saved version '{version}' to cache: {cachePath}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ⚠ Warning: Cache save reported success but file not found at: {cachePath}");
                    }
                }
                catch (Exception cacheEx)
                {
                    // Log but don't fail the download
                    System.Diagnostics.Debug.WriteLine($"[ServerDownloader] ⚠ Cache save failed (non-critical): {cacheEx.Message}");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                OnDownloadProgress(new DownloadProgressEventArgs { Status = $"Error: {ex.Message}", Progress = 0 });
                throw;
            }
        }

        protected virtual void OnDownloadProgress(DownloadProgressEventArgs e)
        {
            DownloadProgress?.Invoke(this, e);
        }
    }

    public class DownloadProgressEventArgs : EventArgs
    {
        public string Status { get; set; } = string.Empty;
        public int Progress { get; set; }
    }

    // JSON models for Minecraft version manifest
    public class VersionManifest
    {
        public VersionInfo[] versions { get; set; } = Array.Empty<VersionInfo>();
    }

    public class VersionInfo
    {
        public string id { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
    }

    public class VersionDetails
    {
        public Downloads? downloads { get; set; }
    }

    public class Downloads
    {
        public ServerDownload? server { get; set; }
    }

    public class ServerDownload
    {
        public string url { get; set; } = string.Empty;
        public long size { get; set; }
    }
}

