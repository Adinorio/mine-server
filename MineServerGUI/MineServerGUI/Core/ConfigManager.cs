using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using MineServerGUI.Models;

namespace MineServerGUI.Core
{
    public class ConfigManager
    {
        private string _serverPropertiesPath;
        private string _whitelistPath;
        private string _opsPath;
        private ServerProfile? _currentProfile;

        public ConfigManager()
        {
            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server");
            _serverPropertiesPath = Path.Combine(baseDir, "server.properties");
            _whitelistPath = Path.Combine(baseDir, "whitelist.json");
            _opsPath = Path.Combine(baseDir, "ops.json");
        }

        public void SetProfile(ServerProfile profile)
        {
            _currentProfile = profile;
            _serverPropertiesPath = Path.Combine(profile.ServerDirectory, "server.properties");
            _whitelistPath = Path.Combine(profile.ServerDirectory, "whitelist.json");
            _opsPath = Path.Combine(profile.ServerDirectory, "ops.json");
        }

        public ServerProperties LoadServerProperties()
        {
            var properties = new ServerProperties();

            if (!File.Exists(_serverPropertiesPath))
            {
                return properties; // Return defaults
            }

            var lines = File.ReadAllLines(_serverPropertiesPath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                    continue;

                var parts = line.Split('=');
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                switch (key)
                {
                    case "max-players":
                        if (int.TryParse(value, out int maxPlayers))
                            properties.MaxPlayers = maxPlayers;
                        break;
                    case "difficulty":
                        properties.Difficulty = value;
                        break;
                    case "gamemode":
                        properties.Gamemode = value;
                        break;
                    case "motd":
                        properties.MOTD = value;
                        break;
                    case "white-list":
                        properties.WhitelistEnabled = value.Equals("true", StringComparison.OrdinalIgnoreCase);
                        break;
                    case "enforce-whitelist":
                        properties.EnforceWhitelist = value.Equals("true", StringComparison.OrdinalIgnoreCase);
                        break;
                }
            }

            return properties;
        }

        public void SaveServerProperties(ServerProperties properties)
        {
            if (!File.Exists(_serverPropertiesPath))
            {
                throw new FileNotFoundException($"server.properties not found: {_serverPropertiesPath}");
            }

            var lines = File.ReadAllLines(_serverPropertiesPath).ToList();
            
            UpdateProperty(lines, "max-players", properties.MaxPlayers.ToString());
            UpdateProperty(lines, "difficulty", properties.Difficulty);
            UpdateProperty(lines, "gamemode", properties.Gamemode);
            UpdateProperty(lines, "motd", properties.MOTD);
            UpdateProperty(lines, "white-list", properties.WhitelistEnabled.ToString().ToLower());
            UpdateProperty(lines, "enforce-whitelist", properties.EnforceWhitelist.ToString().ToLower());

            File.WriteAllLines(_serverPropertiesPath, lines);
        }

        private void UpdateProperty(List<string> lines, string key, string value)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim();
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith(key + "="))
                {
                    lines[i] = $"{key}={value}";
                    return;
                }
            }

            // Property not found, add it
            lines.Add($"{key}={value}");
        }

        public List<string> LoadWhitelist()
        {
            if (!File.Exists(_whitelistPath))
            {
                return new List<string>();
            }

            try
            {
                var json = File.ReadAllText(_whitelistPath);
                var whitelist = JsonConvert.DeserializeObject<List<WhitelistEntry>>(json);
                return whitelist?.Select(e => e.name).ToList() ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public void SaveWhitelist(List<string> players)
        {
            var entries = players.Select(p => new WhitelistEntry { name = p, uuid = Guid.NewGuid().ToString() }).ToList();
            var json = JsonConvert.SerializeObject(entries, Formatting.Indented);
            File.WriteAllText(_whitelistPath, json);
        }

        public void AddToWhitelist(string playerName)
        {
            var whitelist = LoadWhitelist();
            if (!whitelist.Contains(playerName, StringComparer.OrdinalIgnoreCase))
            {
                whitelist.Add(playerName);
                SaveWhitelist(whitelist);
            }
        }

        public void RemoveFromWhitelist(string playerName)
        {
            var whitelist = LoadWhitelist();
            whitelist.RemoveAll(p => p.Equals(playerName, StringComparison.OrdinalIgnoreCase));
            SaveWhitelist(whitelist);
        }
    }
}

