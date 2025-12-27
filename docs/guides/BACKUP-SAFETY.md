# Backup Safety - Running Backups While Server is Active

## ✅ Backups Are Safe!

**You can backup your world while the server is running!**

---

## Why It's Safe

### 1. Read-Only Operation
- **Backup only READS files** - doesn't modify anything
- Server files remain untouched
- No risk of data corruption

### 2. Server Continues Running
- Server keeps processing normally
- Players can keep playing
- No interruption to gameplay

### 3. File System Safety
- Windows handles file copying safely
- Server can read files while backup copies them
- No conflicts or locks

---

## Performance Impact

### Minimal Impact (Usually Unnoticeable)

**What happens:**
- Small increase in disk I/O (reading files)
- Usually **< 1 second** lag spike (if any)
- Most players won't notice

**Why it's minimal:**
- ✅ **Async chunk writes** enabled (non-blocking)
- ✅ Backup reads from disk, not from server memory
- ✅ Modern SSDs handle this easily
- ✅ Backup runs quickly (usually 10-30 seconds)

---

## When to Backup

### ✅ Best Times (Lowest Impact)
- **During low activity** (fewer players)
- **Automatic backups** (every 30 min, spreads load)
- **Anytime** (server handles it fine)

### ⚠️ Avoid (If Possible)
- **During major events** (big builds, many players)
- **Right after server start** (still loading chunks)
- **During world generation** (if generating new areas)

**But even these are usually fine!**

---

## Automatic Backups

**Automatic backups are designed to be safe:**
- Run every 30 minutes (spreads load)
- Quick operation (10-30 seconds)
- Minimal impact on gameplay
- Players usually don't notice

**Setup automatic backups:**
```powershell
.\setup-auto-backup.ps1
```

---

## Manual Backups

**You can backup anytime:**
```powershell
.\backup-world.ps1
```

**Even with:**
- ✅ Players online
- ✅ Active gameplay
- ✅ Server running
- ✅ No downtime needed

---

## What Gets Backed Up

**The backup copies:**
- World files (`server\world\`)
- Player data
- Structures
- All world data

**Does NOT backup:**
- Server logs
- Server configuration (but you should backup `server.properties` separately)
- Plugins (if you add any)

---

## Best Practices

### 1. Regular Backups
- ✅ Automatic backups (every 30 min)
- ✅ Manual backup before major changes
- ✅ Backup before server updates

### 2. Backup Storage
- ✅ Keep backups in `backups\` folder
- ✅ Copy to external drive occasionally
- ✅ Upload to cloud for off-site backup

### 3. Test Restores
- ✅ Test restoring from backup (when server is down)
- ✅ Make sure backups work before you need them

---

## Troubleshooting

### If Backup Causes Lag

**Rare, but possible with very large worlds:**

1. **Reduce backup frequency:**
   - Edit scheduled task
   - Change from 30 min to 1 hour

2. **Backup during off-hours:**
   - Schedule for times with fewer players
   - Use Task Scheduler to set specific times

3. **Check disk space:**
   - Low disk space can slow backups
   - Keep at least 10GB free

---

## Summary

**✅ Backups are SAFE while server is running:**
- No data loss risk
- Minimal performance impact
- Players can keep playing
- Server continues normally

**✅ Automatic backups are recommended:**
- Run every 30 minutes
- Minimal impact
- Peace of mind

**✅ You can backup anytime:**
- Even with players online
- No downtime needed
- Server handles it fine

**Backup with confidence! Your server will be fine! 🎮**





