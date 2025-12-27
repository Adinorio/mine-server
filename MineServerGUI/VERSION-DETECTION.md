# How Version Detection Works

## Overview

When you select a `server.jar` file, the system tries multiple methods to detect the Minecraft server version automatically.

## Detection Methods (In Order)

### Method 1: Filename Pattern ✅ Fastest
- **What it does:** Checks if the filename contains a version number
- **Example:** `server-1.21.11.jar` → detects `1.21.11`
- **Speed:** Instant (no Java needed)
- **Success rate:** High if filename includes version

### Method 2: JAR Manifest File ✅ Fast
- **What it does:** Reads the `META-INF/MANIFEST.MF` file inside the JAR
- **Looks for:** `Implementation-Version`, `Specification-Version`, or `Version` fields
- **Speed:** Very fast (just reading a file)
- **Success rate:** Medium (not all JARs include version in manifest)

### Method 3: version.json File ✅ Fast
- **What it does:** Checks for a `version.json` file inside the JAR
- **Looks for:** `"id": "1.21.11"` pattern
- **Speed:** Very fast (just reading a file)
- **Success rate:** Low (only some JARs include this)

### Method 4: Command Execution ⚠️ Slower
- **What it does:** Runs `java -jar server.jar --version`
- **Parses output:** Looks for version patterns in the output
- **Speed:** 1-2 seconds (requires Java)
- **Success rate:** Medium (depends on server.jar supporting `--version` flag)
- **Note:** Filters out Java version output to avoid false matches

## Validation

All detected versions are validated to ensure they're Minecraft versions, not Java versions:
- ✅ Accepts: `1.7.x`, `1.8.x`, `1.21.11`, etc. (Minecraft versions)
- ❌ Rejects: `1.0`, `1.8`, `1.9` (Java versions)

## User Choice

After detection, you get three options:
1. **Yes** - Use the detected version and continue setup
2. **No** - Select a different file
3. **Cancel** - Cancel the operation

## Why Detection Might Fail

- Java not installed or not in PATH
- Server.jar doesn't support `--version` flag
- Version not included in manifest or filename
- File is corrupted or not a standard Minecraft server JAR
- Detection timeout (max 3 seconds)

## If Detection Fails

You can still use the file! The system will ask if you want to use it anyway, even without version detection.

