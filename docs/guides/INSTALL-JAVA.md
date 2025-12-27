# Installing Java 21 for Minecraft Server

## Current Situation
You have **Java 8** installed, but Minecraft 1.21.1 requires **Java 21 or later**.

## Solution: Install Java 21

You can install Java 21 alongside Java 8 - they won't conflict!

### Step-by-Step Installation

1. **Go to Adoptium (Eclipse Temurin)**
   - Visit: https://adoptium.net/temurin/releases/
   - Or direct link: https://adoptium.net/temurin/releases/?version=21

2. **Download Java 21**
   - Select: **JDK 21** (not JRE)
   - Platform: **Windows**
   - Architecture: **x64**
   - Package Type: **JDK** (or **JDK with Hotspot**)
   - Click **Download**

3. **Install Java 21**
   - Run the downloaded installer (`.msi` file)
   - Follow the installation wizard
   - **Important:** Check "Add to PATH" if the option appears
   - Complete the installation

4. **Verify Installation**
   - Close and reopen PowerShell
   - Run: `java -version`
   - You should see version 21.x.x or higher

## If Java 21 Doesn't Show Up

If `java -version` still shows Java 8 after installing Java 21:

### Option 1: Update PATH (Recommended)
1. Search for "Environment Variables" in Windows
2. Click "Environment Variables"
3. Under "System variables", find `Path`
4. Click "Edit"
5. Move Java 21 path above Java 8 path (or remove Java 8 path)
6. Java 21 path usually looks like: `C:\Program Files\Eclipse Adoptium\jdk-21.x.x-hotspot\bin`
7. Click OK and restart PowerShell

### Option 2: Use Full Java 21 Path
Edit `start-server.ps1` and change:
```powershell
java -Xms$MinMemory -Xmx$MaxMemory -jar server.jar nogui
```
To:
```powershell
"C:\Program Files\Eclipse Adoptium\jdk-21.x.x-hotspot\bin\java.exe" -Xms$MinMemory -Xmx$MaxMemory -jar server.jar nogui
```
(Replace with your actual Java 21 path)

## Quick Check
After installing, run this in PowerShell:
```powershell
java -version
```

You should see something like:
```
openjdk version "21.0.x" ...
```

If you see "1.8" or "8", Java 21 is not being used. Follow the PATH update steps above.

## Alternative: Use Java 17 (Minimum)
If you prefer, Java 17 is the minimum version that works with Minecraft 1.21.1:
- Download from: https://adoptium.net/temurin/releases/?version=17
- Follow the same installation steps

## Need Help?
- Check Java installation: `where java` (shows all Java paths)
- List Java versions: Check `C:\Program Files\Eclipse Adoptium\` or `C:\Program Files\Java\`












