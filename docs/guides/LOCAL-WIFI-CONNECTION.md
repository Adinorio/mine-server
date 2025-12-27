# Local WiFi Connection Guide

## Share This With Friends on Same WiFi

### Connection Information

**IP Address:** `192.168.1.4` (your local IP)
**Port:** `25565`

**Full connection string:**
```
192.168.1.4:25565
```

---

## How Friends Can Connect

### Step-by-Step Instructions

1. **Make sure friend is on the SAME WiFi network**
   - Same router/network as you
   - Not on mobile data or different WiFi

2. **Open Minecraft (TLauncher)**
   - Launch Minecraft version 1.21.11

3. **Click "Multiplayer"**

4. **Click "Direct Connect" or "Add Server"**
   - Direct Connect: Enter IP and connect immediately
   - Add Server: Save server for later

5. **Enter the connection string:**
   ```
   192.168.1.4:25565
   ```
   - No spaces
   - Include the colon `:`
   - Port is `25565`

6. **Click "Join Server" or "Done"**

7. **Wait for connection**
   - First connection may take a few seconds
   - Server will generate chunks if needed

---

## Important Notes

### ✅ Works When:
- Friend is on the **same WiFi network**
- Server is **running** (you started it)
- Windows Firewall allows connections (already configured)

### ❌ Doesn't Work When:
- Friend is on **different WiFi**
- Friend is on **mobile data**
- Friend is at **different location**
- Server is **not running**

---

## If Connection Fails

### Check These:

1. **Is server running?**
   - Look for server window showing "Done!"
   - Server must be started with `.\start-server.ps1`

2. **Same WiFi?**
   - Friend must be on same network
   - Check WiFi name matches

3. **Firewall?**
   - Windows Firewall should allow port 25565
   - Run `.\FIX-FIREWALL-ADMIN.bat` if needed

4. **IP Address Changed?**
   - IP might change after router restart
   - Check current IP: Run `ipconfig` in Command Prompt
   - Look for "IPv4 Address" under your WiFi adapter

---

## Alternative: External Connection

**If friends are NOT on same WiFi:**
- Use **playit.gg tunnel** (external access)
- Share the playit.gg connection string instead
- See `WORKING-CONNECTION-INFO.md` for details

---

## Quick Reference

**For friends on same WiFi:**
```
192.168.1.4:25565
```

**For friends on different network:**
```
Use playit.gg tunnel address
```

---

## Summary

**Local WiFi connection:**
- IP: `192.168.1.4`
- Port: `25565`
- Full: `192.168.1.4:25565`

**Share this with friends on the same WiFi!**





