using System;
using System.Linq;

namespace MineServerGUI.Utilities
{
    public static class VersionHelper
    {
        public static bool IsUpgrade(string currentVersion, string newVersion)
        {
            if (string.IsNullOrEmpty(currentVersion) || string.IsNullOrEmpty(newVersion))
                return false;

            var current = ParseVersion(currentVersion);
            var newVer = ParseVersion(newVersion);

            if (current == null || newVer == null)
                return false;

            // Compare versions
            if (newVer.Major > current.Major)
                return true;
            if (newVer.Major < current.Major)
                return false;

            if (newVer.Minor > current.Minor)
                return true;
            if (newVer.Minor < current.Minor)
                return false;

            return newVer.Patch > current.Patch;
        }

        public static bool IsDowngrade(string currentVersion, string newVersion)
        {
            if (string.IsNullOrEmpty(currentVersion) || string.IsNullOrEmpty(newVersion))
                return false;

            return !IsUpgrade(currentVersion, newVersion) && currentVersion != newVersion;
        }

        public static int GetMajorVersionJump(string currentVersion, string newVersion)
        {
            if (string.IsNullOrEmpty(currentVersion) || string.IsNullOrEmpty(newVersion))
                return 0;

            var current = ParseVersion(currentVersion);
            var newVer = ParseVersion(newVersion);

            if (current == null || newVer == null)
                return 0;

            // Calculate major version difference (e.g., 1.20 -> 1.18 = 2 major versions)
            if (current.Major == newVer.Major)
            {
                return Math.Abs(newVer.Minor - current.Minor);
            }

            return Math.Abs(newVer.Major - current.Major) * 10 + Math.Abs(newVer.Minor - current.Minor);
        }

        public static string GetRequiredJavaVersion(string minecraftVersion)
        {
            if (string.IsNullOrEmpty(minecraftVersion))
                return "Unknown";

            var version = ParseVersion(minecraftVersion);
            if (version == null)
                return "Unknown";

            // Java version requirements based on Minecraft versions
            // 1.18+ requires Java 17+
            if (version.Major == 1 && version.Minor >= 18)
                return "Java 17 or later";

            // 1.17 requires Java 16+
            if (version.Major == 1 && version.Minor == 17)
                return "Java 16 or later";

            // 1.12 - 1.16 requires Java 8+
            if (version.Major == 1 && version.Minor >= 12 && version.Minor <= 16)
                return "Java 8 or later";

            // 1.8 - 1.11 requires Java 8
            if (version.Major == 1 && version.Minor >= 8 && version.Minor <= 11)
                return "Java 8";

            // Older versions
            return "Java 8";
        }

        private static VersionInfo? ParseVersion(string versionString)
        {
            // Remove any non-numeric prefixes/suffixes
            versionString = versionString.Trim();
            
            // Handle formats like "1.21.11", "1.20.1", "1.8.9"
            var parts = versionString.Split('.');
            if (parts.Length < 2)
                return null;

            if (int.TryParse(parts[0], out int major) && int.TryParse(parts[1], out int minor))
            {
                int patch = 0;
                if (parts.Length >= 3)
                {
                    int.TryParse(parts[2], out patch);
                }

                return new VersionInfo { Major = major, Minor = minor, Patch = patch };
            }

            return null;
        }

        private class VersionInfo
        {
            public int Major { get; set; }
            public int Minor { get; set; }
            public int Patch { get; set; }
        }
    }
}

