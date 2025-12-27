# Fixing "getsockopt" Error

## The Error
- **Error:** "getsockopt" or "java.net.SocketException: getsockopt"
- **Meaning:** Network connection cannot be established
- **Common causes:** Firewall blocking, network issues, or tunnel problems

---

## Quick Fixes (Try in Order)

### Fix 1: Use IP Address Instead of Domain ✅ BEST OPTION

**The "getsockopt" error often happens with DNS/domain issues. Use the IP address instead:**

1. **Get the IP address from playit.gg dashboard:**
   - Go to: https://playit.gg/account/agents
   - Find your tunnel
   - Look for the IP address (not the domain)

2. **Have your friend connect using:**
   - `147.185.221.24:37795` (IP address + port)
   - Instead of: `run-generous.gl.joinmc.link:37795` (domain)

3. **Why this works:**
   - IP address doesn't need DNS resolution
   - More reliable connection
   - Bypasses DNS issues

---

### Fix 2: Check Friend's Firewall/Antivirus

**Friend's firewall might be blocking the connection:**

1. **Ask friend to temporarily disable:**
   - Windows Firewall
   - Antivirus software
   - VPN (if using one)

2. **Try connecting again**

3. **If it works:**
   - Add Minecraft/Java to firewall exceptions
   - Or allow port 37795 through firewall

---

### Fix 3: Restart playit.gg Tunnel

**Sometimes the tunnel needs a fresh connection:**

1. **Stop playit.gg** on your computer

2. **Check playit.gg dashboard:**
   - Go to: https://playit.gg/account/agents
   - Make sure tunnel is still configured

3. **Restart playit.gg**

4. **Get new connection info** (IP/port may have changed)

5. **Have friend try connecting again**

---

### Fix 4: Verify Tunnel is Active

**Make sure the tunnel is actually working:**

1. **Check playit.gg dashboard:**
   - Status should be "Active" or "Running"
   - Local address: `127.0.0.1:25565`
   - Tunnel type: "Minecraft Java"

2. **If tunnel shows "Inactive" or "Disconnected":**
   - Restart playit.gg
   - Recreate tunnel if needed

---

### Fix 5: Try Different Connection Methods

**Have friend try these in order:**

1. **IP address with port:**
   - `147.185.221.24:37795`

2. **Domain with port (if IP doesn't work):**
   - `run-generous.gl.joinmc.link:37795`

3. **Alternative IP format (from dashboard):**
   - Check dashboard for exact IP format
   - Some playit.gg addresses use format like: `24.ip.gl.ply.gg:37795`

---

### Fix 6: Check Friend's Network

**Friend's network might be blocking the connection:**

1. **Ask friend to try:**
   - Different WiFi network
   - Mobile hotspot (if available)
   - Different location/network

2. **If it works on different network:**
   - Their home router/firewall is blocking it
   - They need to configure router to allow connection
   - Or use VPN to bypass

---

## Step-by-Step Troubleshooting

**Try this exact order:**

1. ✅ **Have friend use IP address:** `147.185.221.24:37795`
   - This fixes 80% of getsockopt errors

2. ✅ **Check playit.gg is running and tunnel is active**

3. ✅ **Have friend disable firewall/antivirus temporarily** (to test)

4. ✅ **Restart playit.gg and get fresh connection info**

5. ✅ **Have friend try different network** (if possible)

---

## Common Causes

### Cause 1: DNS Resolution Failure
**Fix:** Use IP address instead of domain

### Cause 2: Firewall Blocking Connection
**Fix:** Disable firewall temporarily, or add exception

### Cause 3: Tunnel Not Active
**Fix:** Restart playit.gg, check dashboard

### Cause 4: Friend's Router/ISP Blocking
**Fix:** Try different network, or use VPN

### Cause 5: Network Timeout
**Fix:** Use IP address, check connection stability

---

## Summary

**To fix "getsockopt" error:**

1. ✅ **Use IP address:** `147.185.221.24:37795` (instead of domain)
2. ✅ **Check friend's firewall/antivirus** isn't blocking
3. ✅ **Verify playit.gg tunnel is active**
4. ✅ **Restart playit.gg if needed**
5. ✅ **Try different network** (if possible)

**The IP address method usually fixes this error!**

