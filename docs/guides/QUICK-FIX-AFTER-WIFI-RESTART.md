# Quick Fix: Server Not Working After WiFi Restart

## Problem
After restarting WiFi, the server might not be accessible even though it's running.

## Why This Happens
- Windows Firewall might reset after network changes
- The server is still running, but firewall is blocking connections

## Quick Fix (2 minutes)

### Step 1: Open PowerShell as Administrator
1. Press `Windows Key + X`
2. Click **"Windows PowerShell (Admin)"** or **"Terminal (Admin)"**
3. Click **"Yes"** when asked for permission

### Step 2: Navigate to Server Folder
```powershell
cd C:\Users\LAPIS\Documents\Github\mine-server
```

### Step 3: Run Firewall Fix
```powershell
.\fix-firewall.ps1
```

**OR** run this command directly:
```powershell
New-NetFirewallRule -DisplayName "Minecraft Server - Port 25565" -Direction Inbound -LocalPort 25565 -Protocol TCP -Action Allow
```

### Step 4: Test Connection
- **Local IP:** `192.168.1.6:25565`
- **From same WiFi:** Should work now!

---

## Verify Server is Running

**Check if server is running:**
```powershell
Get-Process -Name "java" -ErrorAction SilentlyContinue
```

**If no Java process, start server:**
```powershell
.\start-server.ps1
```

---

## Current Status

✅ **Server:** Running (Java process active)  
✅ **Port 25565:** Listening  
✅ **Your IP:** 192.168.1.6  
⚠️ **Firewall:** Needs to be fixed (blocking connections)

---

## After Fixing Firewall

**Connect using:**
- **Local network (same WiFi):** `192.168.1.6:25565`
- **From your computer:** `localhost:25565` or `127.0.0.1:25565`

---

## If Still Not Working

1. **Restart the server:**
   ```powershell
   .\stop-server.ps1
   .\start-server.ps1
   ```

2. **Check IP address changed:**
   - WiFi restart might have changed your IP
   - Run `ipconfig` to check new IP
   - Use the new IP address

3. **Verify firewall rule:**
   ```powershell
   Get-NetFirewallRule -DisplayName "*Minecraft*"
   ```

---

## Summary

**The issue:** Firewall blocking after WiFi restart  
**The fix:** Run `fix-firewall.ps1` as Administrator  
**Time:** 2 minutes  
**Result:** Server accessible again!








