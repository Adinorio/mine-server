# Fixing Firewall Blocking Connections

## Two Places Firewall Can Block

1. **Your Computer (Server Side)** - Blocks incoming connections
2. **Friend's Computer (Client Side)** - Blocks outgoing connections to your server

---

## Part 1: Fix Server-Side Firewall (Your Computer) ✅

### Quick Fix (Easiest Method)

**Run this script as Administrator:**

```powershell
# Right-click PowerShell and select "Run as Administrator"
.\FIX-FIREWALL-ADMIN.bat
```

**OR manually run:**

```powershell
# Run PowerShell as Administrator
New-NetFirewallRule -DisplayName "Minecraft Server - Port 25565" `
    -Direction Inbound `
    -LocalPort 25565 `
    -Protocol TCP `
    -Action Allow `
    -Description "Allows Minecraft Server connections on port 25565"
```

---

### Manual Windows Firewall Fix (Server Side)

1. **Open Windows Defender Firewall:**
   - Press `Windows Key + R`
   - Type: `wf.msc`
   - Press Enter

2. **Click "Inbound Rules"** (left side)

3. **Click "New Rule..."** (right side)

4. **Rule Type:**
   - Select "Port"
   - Click Next

5. **Protocol and Ports:**
   - Select "TCP"
   - Select "Specific local ports"
   - Enter: `25565`
   - Click Next

6. **Action:**
   - Select "Allow the connection"
   - Click Next

7. **Profile:**
   - Check all: Domain, Private, Public
   - Click Next

8. **Name:**
   - Name: `Minecraft Server - Port 25565`
   - Click Finish

**✅ Done! Your server firewall is now configured.**

---

## Part 2: Fix Friend's Firewall (Client Side) ⚠️

**Your friend needs to do this on their computer.**

### Option A: Allow Java/Minecraft Through Firewall (Recommended)

1. **Open Windows Defender Firewall:**
   - Press `Windows Key + R`
   - Type: `wf.msc`
   - Press Enter

2. **Click "Outbound Rules"** (left side)

3. **Click "New Rule..."** (right side)

4. **Rule Type:**
   - Select "Program"
   - Click Next

5. **Program:**
   - Select "This program path:"
   - Browse to Java or Minecraft executable:
     - Java: Usually `C:\Program Files\Java\jre-XX\bin\java.exe`
     - TLauncher: Where they installed it (e.g., `C:\Users\USERNAME\AppData\Roaming\TLauncher\`)
   - Or select "All programs" (less secure but works)
   - Click Next

6. **Action:**
   - Select "Allow the connection"
   - Click Next

7. **Profile:**
   - Check all: Domain, Private, Public
   - Click Next

8. **Name:**
   - Name: `Minecraft - Allow Outbound`
   - Click Finish

---

### Option B: Temporarily Disable Firewall (Quick Test)

**⚠️ Only for testing! Re-enable after testing.**

1. **Open Windows Security:**
   - Press `Windows Key + I`
   - Go to "Privacy & Security" → "Windows Security"
   - Click "Firewall & network protection"

2. **Turn off firewall:**
   - Click on each network (Domain, Private, Public)
   - Turn off "Microsoft Defender Firewall"

3. **Test connection**

4. **Re-enable firewall** after testing

5. **If it worked:** Use Option A above to permanently allow

---

### Option C: Allow Port Through Firewall (Advanced)

**Have friend create outbound rule for port:**

1. **Open Windows Defender Firewall** (`wf.msc`)

2. **Click "Outbound Rules"** → "New Rule..."

3. **Rule Type:** Select "Port" → Next

4. **Protocol:** TCP, Specific ports: `37795` (or the port your playit.gg uses)

5. **Action:** Allow the connection → Next

6. **Profile:** Check all → Next

7. **Name:** "Minecraft - Allow Port 37795" → Finish

---

## Part 3: Check Antivirus Software ⚠️

**Antivirus can also block connections!**

### Common Antivirus Programs:

- **Windows Defender** (built-in) - Usually covered by firewall fix above
- **Avast, AVG, Norton, McAfee, Kaspersky** - Check their settings

### How to Fix:

1. **Temporarily disable antivirus** (to test)
2. **Add Minecraft/Java to exceptions:**
   - Look for "Exceptions", "Allowed Apps", or "Exclusions"
   - Add Java executable or Minecraft folder

---

## Quick Checklist

### For You (Server Owner):
- ✅ Run `.\FIX-FIREWALL-ADMIN.bat` as Administrator
- ✅ Verify rule exists: Check Windows Firewall → Inbound Rules
- ✅ Check antivirus isn't blocking

### For Your Friend (Client):
- ✅ Temporarily disable firewall (to test if that's the issue)
- ✅ Add Java/Minecraft to firewall exceptions
- ✅ Check antivirus isn't blocking
- ✅ Disable VPN if using one

---

## Verify Firewall Rule is Working

### Check Server-Side Firewall Rule:

```powershell
# Run in PowerShell
Get-NetFirewallRule -DisplayName "*Minecraft*"
```

Should show:
- `Minecraft Server - Port 25565` with Action: Allow, Enabled: True

---

## Common Issues

### Issue 1: "Access Denied" When Running Script
**Fix:** Run PowerShell/Command Prompt as Administrator

### Issue 2: Friend's Router Firewall Blocking
**Fix:** Friend's home router may block outbound connections
- They may need to configure router settings
- Or try connecting from different network (mobile hotspot)

### Issue 3: VPN Blocking Connection
**Fix:** Disable VPN temporarily to test

### Issue 4: Corporate/School Network Blocking
**Fix:** Network admin may block gaming connections
- Try connecting from home network instead
- Or use mobile hotspot

---

## Summary

**Server Side (Your Computer):**
1. Run `.\FIX-FIREWALL-ADMIN.bat` as Administrator
2. Done! ✅

**Client Side (Friend's Computer):**
1. Temporarily disable firewall to test
2. If it works, add Minecraft/Java to firewall exceptions
3. Check antivirus settings
4. Disable VPN if using

**Most common fix:** Your friend needs to allow Java/Minecraft through their firewall!

