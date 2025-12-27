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
        private List<ServerProfile> _profiles = new List<ServerProfile>();
        private ServerProfile? _currentProfile;

        public ServerProfileManager()
        {
            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
            _profilesDirectory = Path.Combine(baseDir, "server-profiles");
            _profilesConfigPath = Path.Combine(_profilesDirectory, "profiles.json");
            
            if (!Directory.Exists(_profilesDirectory))
            {
                Directory.CreateDirectory(_profilesDirectory);
            }
            
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
                    // Load the first profile as current (or last used)
                    _currentProfile = _profiles.FirstOrDefault();
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
    }
}

