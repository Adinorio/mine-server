# Quick Start Guide - Everything You Need

## 🚀 Daily Startup Checklist

**To start your server (3 steps):**

1. **Start Java Server:**
   ```powershell
   .\start-server.ps1
   ```

2. **Start GeyserMC (for Bedrock players):**
   ```powershell
   .\start-geyser.ps1
   ```

3. **Keep playit.gg running:**
   - Make sure playit.gg application is open
   - Check both tunnels are active

## 📋 Connection Addresses

### Java Players:
- **Your playit.gg Java tunnel address**
- Example: `run-generous.gl.joinmc.link:37795`
- Check playit.gg dashboard for current address

### Bedrock Players:
- **Your playit.gg Bedrock tunnel address**
- Example: `believe-interaction.gl.at.ply.gg:23534`
- Or IP: `147.185.221.16:23534`
- Check playit.gg dashboard for current address

## ✅ Optional: Setup Automatic Backups

**Run once to setup auto-backups:**
```powershell
.\setup-auto-backup.ps1
```

This backs up your world every 30 minutes automatically!

## 🔧 Useful Commands

### Start Server
```powershell
.\start-server.ps1
```

### Start GeyserMC
```powershell
.\start-geyser.ps1
```

### Manual Backup
```powershell
.\backup-world.ps1
```

### Check Status
```powershell
.\check-server-status.ps1
```

### Update Server Version
```powershell
.\update-server.ps1
```

## 📚 Important Documents

- **Bedrock Setup:** `BEDROCK-CROSSPLAY.md`
- **Optimization:** `GEYSER-OPTIMIZATION.md`
- **Balance:** `CROSSPLAY-BALANCE.md`
- **Backups:** `BACKUP-GUIDE.md`
- **Troubleshooting:** `FIX-BEDROCK-CHUNK-LOADING.md`

## ⚠️ Common Issues

### Bedrock Players Can't Connect
- Make sure GeyserMC is running
- Check playit.gg Bedrock tunnel is active
- Verify address: `147.185.221.16:23534`

### Chunks Not Loading (Bedrock)
- Tell players to set render distance to **12 chunks** in MCPE settings
- Restart GeyserMC if needed

### Java Players Can't Connect
- Check server is running
- Verify playit.gg Java tunnel is active
- Check connection address

## 🎮 Current Server Settings

- **View Distance:** 12 chunks
- **Simulation Distance:** 10 chunks
- **Max Players:** 8
- **Gamemode:** Survival
- **Difficulty:** Normal
- **PvP:** Enabled
- **Version:** 1.21.11

## 📞 Support

If you need help:
1. Check the troubleshooting guides
2. Look in `server\logs\latest.log` for errors
3. Check `geyser\logs\latest.log` for Geyser issues

---

**Everything is ready! Just start server and GeyserMC! 🚀**



