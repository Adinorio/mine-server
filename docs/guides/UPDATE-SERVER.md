# How to Update Your Minecraft Server

This guide explains how to update your server to a newer version of Minecraft.

## Quick Update (Recommended)

Update to the latest version:

```powershell
.\update-server.ps1
```

This will:
- Check for the latest Minecraft version
- Backup your current server.jar
- Download and install the latest version

## Update to Specific Version

Update to a specific version (e.g., 1.21.1):

```powershell
.\update-server.ps1 -Version "1.21.1"
```

## Important: Before Updating

### 1. **Backup Your World!**

Always backup your world before updating:

```powershell
.\backup-world.ps1
```

Or manually:
```powershell
Copy-Item -Path "server\world" -Destination "backups\world-$(Get-Date -Format 'yyyy-MM-dd-HHmmss')" -Recurse
```

### 2. **Check Version Compatibility**

- **Upgrading** (newer version): Usually safe, but test first
- **Downgrading** (older version): **DANGEROUS** - may corrupt your world!
  - Only downgrade if you have a backup from that version
  - World format changes between versions

### 3. **Check Java Version**

Newer Minecraft versions may require newer Java:
- Minecraft 1.21+ requires Java 21+
- Check: `java -version`

## Update Process

1. **Stop your server:**
   - Press `Ctrl+C` in server window
   - Or run: `.\stop-server.ps1`

2. **Run update script:**
   ```powershell
   .\update-server.ps1
   ```

3. **Follow prompts:**
   - Script will show available versions
   - Confirm the update
   - Script will backup and download new version

4. **Start server:**
   ```powershell
   .\start-server.ps1
   ```

5. **First start:**
   - May take longer (converting world format)
   - Check for errors in logs
   - Test that world loads correctly

## Manual Update (If Script Fails)

1. **Backup current server.jar:**
   ```powershell
   Copy-Item server\server.jar server\server.jar.backup
   ```

2. **Download new version:**
   - Go to: https://www.minecraft.net/en-us/download/server
   - Or: https://mcversions.net/
   - Download server.jar for desired version

3. **Replace server jar:**
   - Place new server.jar in `server\` folder
   - Replace the old one

4. **Start server and test**

## Troubleshooting

### Server Won't Start After Update

1. **Check Java version:**
   - New version might need newer Java
   - Run `java -version`

2. **Check logs:**
   - Look in `server\logs\latest.log`
   - Look for error messages

3. **Restore backup:**
   - Replace `server\server.jar` with backup
   - Restore world from backup if corrupted

### World Won't Load

1. **Check if world format changed:**
   - Some updates change world format
   - Server should convert automatically on first start

2. **Check logs for conversion errors:**
   - Look in `server\logs\latest.log`

3. **Restore world backup:**
   - Restore from your backup
   - Try updating again

### Version Not Found

If the script says version not found:
- Check version number spelling (e.g., "1.21.1" not "1.21")
- Use "latest" to get most recent version
- Check https://mcversions.net/ for available versions

## GeyserMC Compatibility

If you're using GeyserMC for Bedrock crossplay:
- GeyserMC usually works with newer Minecraft versions
- Update GeyserMC if needed: https://geysermc.org/download
- Check GeyserMC compatibility for specific versions

## Rollback (Go Back to Old Version)

If update causes problems:

1. **Stop server**

2. **Restore old server.jar:**
   ```powershell
   # Find backup in server\versions\
   Copy-Item server\versions\server-[timestamp].jar server\server.jar
   ```

3. **Restore world backup if needed**

4. **Start server**

**Warning:** Downgrading may corrupt your world! Only rollback if you have a backup from the old version.

## Checking Current Version

To check what version your server is:

1. Start the server
2. Look at the first few lines in the console
3. Or check `server\logs\latest.log` - first line shows version

## Summary

✅ **Safe:** Updating to newer version (with backup)
✅ **Safe:** Updating from snapshot to release
⚠️ **Risky:** Downgrading (may corrupt world)
❌ **Don't:** Update without backing up first

Always backup before updating!



