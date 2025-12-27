# Backup Guide

## Current Backup Status

**Backups are currently MANUAL** - you need to run them yourself.

---

## Quick Backup (Manual)

**Run this command:**
```powershell
.\backup-world.ps1
```

**Or use the new auto-backup script:**
```powershell
.\auto-backup.ps1
```

**What it does:**
- Creates a backup in `backups\` folder
- Names it with timestamp: `world-2025-12-18-203045`
- Keeps last 10 backups (auto-deletes old ones)

---

## Automatic Backups (Recommended)

### Setup Automatic Backups

**Run this once:**
```powershell
.\setup-auto-backup.ps1
```

**What it does:**
- Creates a Windows Scheduled Task
- Backs up every **30 minutes** automatically
- Keeps last **20 backups**
- Runs in background (no need to remember)

---

## Backup Frequency Recommendations

### For Active Servers (Daily Play)
- **Every 30 minutes** - Recommended (automatic)
- **Every hour** - Good balance
- **Every 2 hours** - Less frequent but still safe

### For Casual Servers (Few Times a Week)
- **Every 6 hours** - Good for casual play
- **Daily** - Minimum for active servers
- **Before major events** - Manual backup before big builds

### For Testing/Development
- **Every hour** - Good for testing
- **Before testing new features** - Manual backup

---

## Backup Storage

**Location:** `backups\` folder

**Format:** `world-YYYY-MM-DD-HHmmss`

**Example:**
```
backups\
  world-2025-12-18-120000\
  world-2025-12-18-123000\
  world-2025-12-18-130000\
  ...
```

**Storage size:**
- Each backup: ~50-500 MB (depends on world size)
- 20 backups: ~1-10 GB total
- Old backups auto-deleted (keeps last 20)

---

## Restore from Backup

### If World Gets Corrupted

1. **Stop the server:**
   ```powershell
   .\stop-server.ps1
   ```

2. **Delete corrupted world:**
   ```powershell
   Remove-Item -Path "server\world" -Recurse -Force
   ```

3. **Copy backup to world folder:**
   ```powershell
   Copy-Item -Path "backups\world-2025-12-18-120000" -Destination "server\world" -Recurse
   ```

4. **Start server:**
   ```powershell
   .\start-server.ps1
   ```

---

## Manual Backup Options

### Option 1: Quick Backup Script
```powershell
.\backup-world.ps1
```
- Keeps last 10 backups
- Shows progress

### Option 2: Auto Backup Script
```powershell
.\auto-backup.ps1
```
- Keeps last 20 backups
- Silent (for scheduled tasks)

### Option 3: Manual Copy
```powershell
Copy-Item -Path "server\world" -Destination "backups\world-$(Get-Date -Format 'yyyy-MM-dd-HHmmss')" -Recurse
```

---

## Check Backup Status

**View all backups:**
```powershell
Get-ChildItem -Path "backups" -Directory | Sort-Object LastWriteTime -Descending
```

**Check backup size:**
```powershell
Get-ChildItem -Path "backups" -Directory | ForEach-Object { 
    $size = (Get-ChildItem $_.FullName -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
    [PSCustomObject]@{Name=$_.Name; SizeMB=[math]::Round($size, 2); Date=$_.LastWriteTime}
} | Format-Table -AutoSize
```

---

## Best Practices

1. **Enable automatic backups** - Don't rely on remembering
2. **Keep backups off-site** - Copy to cloud/external drive occasionally
3. **Test restore** - Make sure backups work before you need them
4. **Backup before updates** - Manual backup before server updates
5. **Monitor disk space** - Backups take space, clean old ones if needed

---

## Summary

**Current setup:**
- ❌ Manual backups only (you need to run them)
- ✅ Script available: `backup-world.ps1`
- ✅ Can setup automatic: `setup-auto-backup.ps1`

**Recommended:**
- ✅ Setup automatic backups (every 30 minutes)
- ✅ Keep last 20 backups
- ✅ Manual backup before major changes

**Run this to setup automatic backups:**
```powershell
.\setup-auto-backup.ps1
```





