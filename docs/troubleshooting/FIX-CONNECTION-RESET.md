# Fixing "Connection Reset" Error with playit.gg

## The Error
- **Error:** "Internal Exception: java.net.SocketException: Connection reset"
- **Meaning:** Connection starts but then gets closed/reset
- **Common cause:** playit.gg tunnel instability or configuration issue

## Quick Fixes (Try in Order)

### Fix 1: Restart Everything

**Connection reset often happens when tunnel needs restart:**

1. **Stop Minecraft server:**
   - Press `Ctrl+C` in server window
   - Or run: `.\stop-server.ps1`

2. **Close playit.gg completely**

3. **Restart in order:**
   ```powershell
   # 1. Start server first
   .\start-server.ps1
   
   # 2. Wait for server to fully start (see "Done!")
   # 3. Then start playit.gg
   ```

4. **Check new connection info:**
   - Port might have changed
   - Use new IP:PORT from dashboard

5. **Try connecting again**

---

### Fix 2: Recreate Tunnel in playit.gg

**Sometimes the tunnel needs to be recreated:**

1. **Go to playit.gg dashboard:**
   - https://playit.gg/account/agents

2. **Delete the old tunnel:**
   - Click on your "Minecraft Java" tunnel
   - Delete/Remove it

3. **Create a new tunnel:**
   - Click "Add Tunnel"
   - Select "Minecraft Java"
   - Local address: `127.0.0.1:25565`
   - Save

4. **Get new connection info:**
   - Note the new IP and PORT
   - Use these to connect

5. **Try connecting with new info**

---

### Fix 3: Check Tunnel Configuration

**Make sure tunnel is configured correctly:**

1. **In playit.gg dashboard, check:**
   - **Local Address:** Should be `127.0.0.1:25565`
   - **Tunnel Type:** Should be "Minecraft Java" (not generic TCP)
   - **Status:** Should be "Active" or "Running"

2. **If wrong, update it:**
   - Edit tunnel settings
   - Set local address to `127.0.0.1:25565`
   - Make sure type is "Minecraft Java"

---

### Fix 4: Try Different Connection Method

**Sometimes the connection method matters:**

1. **Try IP address:**
   - `147.185.221.24:37795`

2. **Try domain:**
   - `run-generous.gl.joinmc.link:37795`

3. **Try without port (if playit.gg supports it):**
   - `147.185.221.24`
   - (Usually doesn't work, but worth trying)

---

### Fix 5: Check Server Logs

**See if server is receiving connections:**

1. **Look at server console:**
   - When you try to connect, does server show anything?
   - Any error messages?

2. **If server shows connection attempts:**
   - Tunnel is working
   - Issue might be server-side

3. **If server shows nothing:**
   - Tunnel might not be forwarding properly
   - Try recreating tunnel

---

## Common Causes

### Cause 1: Tunnel Not Properly Connected
**Fix:** Restart playit.gg and recreate tunnel

### Cause 2: Server Not Ready
**Fix:** Make sure server fully started before connecting

### Cause 3: Wrong Tunnel Type
**Fix:** Make sure tunnel is "Minecraft Java" not generic TCP

### Cause 4: playit.gg Service Issue
**Fix:** Wait a few minutes and try again, or use alternative service

---

## Alternative: Use Tunnely Instead

**If playit.gg keeps having issues:**

- **Tunnely** is more reliable for Minecraft
- See: `TUNNELY-SETUP.md`
- Made specifically for Minecraft servers
- Often more stable than playit.gg

---

## Step-by-Step Troubleshooting

**Try this exact order:**

1. **Stop everything:**
   - Stop server (Ctrl+C)
   - Close playit.gg

2. **Start server:**
   ```powershell
   .\start-server.ps1
   ```

3. **Wait for server to fully start:**
   - Look for "Done!" message
   - Server should say it's ready

4. **Start playit.gg:**
   - Let it connect
   - Wait for tunnel to be active

5. **Check dashboard:**
   - Get new IP:PORT
   - Make sure tunnel is active

6. **Try connecting:**
   - Use IP:PORT from dashboard
   - Try both IP and domain

7. **If still doesn't work:**
   - Recreate tunnel in dashboard
   - Try Tunnely instead

---

## Summary

**To fix "Connection reset":**

1. ✅ Restart server and playit.gg
2. ✅ Recreate tunnel in playit.gg
3. ✅ Check tunnel configuration
4. ✅ Try different connection methods
5. ✅ Consider Tunnely if playit.gg keeps failing

**Most common fix: Restart everything and recreate tunnel!**








