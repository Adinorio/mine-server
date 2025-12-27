using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using MineServerGUI.Models;

namespace MineServerGUI.Core
{
    public class ServerProfileManager
    {
        private readonly string _profilesDirectory;
        private readonly string _profilesConfigPath;
        private readonly string _configPath;
        private List<ServerProfile> _profiles = new List<ServerProfile>();
        private ServerProfile? _currentProfile;
        private string? _lastSelectedProfileId;

        public ServerProfileManager()
        {
            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
            _profilesDirectory = Path.Combine(baseDir, "server-profiles");
            _profilesConfigPath = Path.Combine(_profilesDirectory, "profiles.json");
            _configPath = Path.Combine(_profilesDirectory, "profile-config.json");
            
            if (!Directory.Exists(_profilesDirectory))
            {
                Directory.CreateDirectory(_profilesDirectory);
            }
            
            LoadLastSelectedProfile();
            LoadProfiles();
        }

        public List<ServerProfile> Profiles => _profiles;
        public ServerProfile? CurrentProfile => _currentProfile;

        private void LoadProfiles()
        {
            if (!File.Exists(_profilesConfigPath))
            {
                _profiles = new List<ServerProfile>();
                // Create default profile if none exist
                CreateDefaultProfile();
                return;
            }

            try
            {
                var json = File.ReadAllText(_profilesConfigPath);
                _profiles = JsonConvert.DeserializeObject<List<ServerProfile>>(json) ?? new List<ServerProfile>();
                
                // If no profiles exist, create default
                if (_profiles.Count == 0)
                {
                    CreateDefaultProfile();
                }
                else
                {
                    // Try to load the last selected profile first
                    if (!string.IsNullOrEmpty(_lastSelectedProfileId))
                    {
                        var lastSelected = _profiles.FirstOrDefault(p => p.Id == _lastSelectedProfileId);
                        if (lastSelected != null && File.Exists(lastSelected.ServerJarPath))
                        {
                            _currentProfile = lastSelected;
                            System.Diagnostics.Debug.WriteLine($"[ServerProfileManager] Loaded last selected profile: {lastSelected.Name}");
                            return;
                        }
                    }
                    
                    // If last selected doesn't exist or is invalid, select the best profile:
                    // 1. Prefer profiles with valid JAR files
                    // 2. Prefer profiles with meaningful names (not single characters)
                    // 3. Prefer most recently modified
                    ServerProfile? bestProfile = null;
                    
                    // First, try to find a profile with a valid JAR and meaningful name
                    foreach (var profile in _profiles)
                    {
                        if (File.Exists(profile.ServerJarPath))
                        {
                            // Prefer profiles with names longer than 1 character (not "t", "a", etc.)
                            if (profile.Name.Length > 1)
                            {
                                if (bestProfile == null || 
                                    profile.LastModifiedDate > bestProfile.LastModifiedDate)
                                {
                                    bestProfile = profile;
                                }
                            }
                        }
                    }
                    
                    // If no good profile found, try any profile with valid JAR
                    if (bestProfile == null)
                    {
                        bestProfile = _profiles.FirstOrDefault(p => File.Exists(p.ServerJarPath));
                    }
                    
                    // If still no profile, use the most recently modified
                    if (bestProfile == null)
                    {
                        bestProfile = _profiles.OrderByDescending(p => p.LastModifiedDate).FirstOrDefault();
                    }
                    
                    // Fallback to first profile
                    _currentProfile = bestProfile ?? _profiles.FirstOrDefault();
                    
                    // Save the selected profile as last selected
                    if (_currentProfile != null)
                    {
                        SaveLastSelectedProfile(_currentProfile.Id);
                    }
                }
            }
            catch
            {
                _profiles = new List<ServerProfile>();
                CreateDefaultProfile();
            }
        }

        private void CreateDefaultProfile()
        {
            var defaultProfile = new ServerProfile
            {
                Name = "Default Server",
                Version = "Unknown",
                ServerDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server"),
                ServerJarPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server", "server.jar"),
                Description = "Default server profile"
            };
            
            _profiles.Add(defaultProfile);
            _currentProfile = defaultProfile;
            SaveProfiles();
        }

        public void SaveProfiles()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_profiles, Formatting.Indented);
                File.WriteAllText(_profilesConfigPath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save profiles: {ex.Message}", ex);
            }
        }

        public ServerProfile CreateProfile(string name, string version, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Profile name cannot be empty", nameof(name));
            }

            if (_profiles.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"A profile with the name '{name}' already exists");
            }

            // Create directory for this profile
            var profileDir = Path.Combine(_profilesDirectory, SanitizeFileName(name));
            if (!Directory.Exists(profileDir))
            {
                Directory.CreateDirectory(profileDir);
            }

            var profile = new ServerProfile
            {
                Name = name,
                Version = version,
                ServerDirectory = profileDir,
                ServerJarPath = Path.Combine(profileDir, "server.jar"),
                Description = description,
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now
            };

            _profiles.Add(profile);
            SaveProfiles();
            
            // Set as current and remember selection
            _currentProfile = profile;
            SaveLastSelectedProfile(profile.Id);
            
            return profile;
        }

        public void DeleteProfile(string profileId)
        {
            var profile = _profiles.FirstOrDefault(p => p.Id == profileId);
            if (profile == null)
            {
                throw new ArgumentException($"Profile with ID '{profileId}' not found");
            }

            // Don't allow deleting if it's the only one
            if (_profiles.Count == 1)
            {
                throw new InvalidOperationException("Cannot delete the last remaining profile");
            }

            // If deleting current profile, switch to another (should be handled by caller, but safety check)
            if (_currentProfile?.Id == profileId)
            {
                var otherProfile = _profiles.FirstOrDefault(p => p.Id != profileId);
                if (otherProfile != null)
                {
                    _currentProfile = otherProfile;
                }
                else
                {
                    throw new InvalidOperationException("Cannot delete the last remaining profile");
                }
            }

            _profiles.Remove(profile);
            SaveProfiles();

            // Optionally delete the profile directory (with confirmation in UI)
            // For now, we'll keep the directory in case user wants to recover
        }

        public void SetCurrentProfile(string profileId)
        {
            var profile = _profiles.FirstOrDefault(p => p.Id == profileId);
            if (profile == null)
            {
                throw new ArgumentException($"Profile with ID '{profileId}' not found");
            }

            _currentProfile = profile;
            SaveLastSelectedProfile(profileId); // Remember this selection
            SaveProfiles();
        }

        public void UpdateProfile(ServerProfile profile)
        {
            var existing = _profiles.FirstOrDefault(p => p.Id == profile.Id);
            if (existing == null)
            {
                throw new ArgumentException($"Profile with ID '{profile.Id}' not found");
            }

            existing.Name = profile.Name;
            existing.Version = profile.Version;
            existing.Description = profile.Description;
            existing.MinMemory = profile.MinMemory;
            existing.MaxMemory = profile.MaxMemory;
            existing.LastModifiedDate = DateTime.Now;
            
            SaveProfiles();
        }

        private string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
                .TrimEnd('.');
        }

        /// <summary>
        /// Loads the last selected profile ID from config
        /// </summary>
        private void LoadLastSelectedProfile()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var json = File.ReadAllText(_configPath);
                    var config = JsonConvert.DeserializeObject<ProfileConfig>(json);
                    if (config != null && !string.IsNullOrEmpty(config.LastSelectedProfileId))
                    {
                        _lastSelectedProfileId = config.LastSelectedProfileId;
                        System.Diagnostics.Debug.WriteLine($"[ServerProfileManager] Loaded last selected profile ID: {_lastSelectedProfileId}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ServerProfileManager] Failed to load last selected profile: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves the last selected profile ID to config
        /// </summary>
        private void SaveLastSelectedProfile(string profileId)
        {
            try
            {
                _lastSelectedProfileId = profileId;
                var config = new ProfileConfig
                {
                    LastSelectedProfileId = profileId
                };
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(_configPath, json);
                System.Diagnostics.Debug.WriteLine($"[ServerProfileManager] Saved last selected profile ID: {profileId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ServerProfileManager] Failed to save last selected profile: {ex.Message}");
            }
        }

        private class ProfileConfig
        {
            public string? LastSelectedProfileId { get; set; }
        }
    }
}

