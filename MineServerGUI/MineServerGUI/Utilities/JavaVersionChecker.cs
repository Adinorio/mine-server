using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MineServerGUI.Utilities
{
    public static class JavaVersionChecker
    {
        public static int GetJavaMajorVersion(string? javaPath = null)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = javaPath ?? "java",
                    Arguments = "-version",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process == null)
                    return 0;

                process.WaitForExit(2000);
                var output = process.StandardError.ReadToEnd() + process.StandardOutput.ReadToEnd();

                // Parse Java version
                // Java 8 format: "java version "1.8.0_401""
                // Java 9+ format: "openjdk version "21.0.1""
                var match = Regex.Match(output, @"version\s+[""']?(\d+)\.(\d+)");
                if (match.Success)
                {
                    var major = int.Parse(match.Groups[1].Value);
                    var minor = int.Parse(match.Groups[2].Value);

                    // Java 8 uses "1.8" format, so major=1, minor=8
                    if (major == 1)
                    {
                        return minor; // Return 8 for Java 8
                    }

                    return major; // Return 17, 21, etc. for newer versions
                }
            }
            catch
            {
                // Failed to detect
            }

            return 0;
        }

        public static bool IsJavaVersionCompatible(int javaMajorVersion, string requiredJava)
        {
            if (javaMajorVersion == 0)
                return false; // Java not found

            // Parse required Java version
            if (requiredJava.Contains("Java 17") || requiredJava.Contains("17"))
            {
                return javaMajorVersion >= 17;
            }
            if (requiredJava.Contains("Java 16") || requiredJava.Contains("16"))
            {
                return javaMajorVersion >= 16;
            }
            if (requiredJava.Contains("Java 8") || requiredJava.Contains("8"))
            {
                return javaMajorVersion >= 8;
            }

            // Default: assume Java 8+ is required
            return javaMajorVersion >= 8;
        }
    }
}

