# Fixing playit.gg Slowness

## Why is playit.gg Slow?

**Common causes:**
1. **Free tier bandwidth limits** - playit.gg may throttle free accounts
2. **Tunnel region changed** - Different regions have different speeds
3. **playit.gg server load** - High traffic on their servers
4. **Network congestion** - Your internet or their servers are busy
5. **Tunnel needs recreation** - Old tunnel might be on a slow server

---

## Quick Fixes (Try in Order)

### Fix 1: Change Tunnel Region

**Different regions have different speeds:**

1. **Go to playit.gg dashboard:**
   - https://playit.gg/account/agents
   - Click on your tunnel

2. **Change region:**
   - Look for "Region" setting
   - Try: **Singapore1** (closest to Philippines)
   - Or: **Tokyo1** (also close)
   - Avoid: **Global Anycast** (can be slower)

3. **Save and wait for tunnel to reconnect**

4. **Get new connection info:**
   - Port might change
   - Use new IP:PORT

---

### Fix 2: Restart Everything

**Sometimes a fresh start helps:**

1. **Stop Minecraft server:**
   - Press `Ctrl+C` in server window

2. **Close playit.gg completely**

3. **Wait 30 seconds**

4. **Restart in order:**
   ```powershell
   # 1. Start server first
   .\start-server.ps1
   ```
   - Wait for server to fully start (see "Done!")
   - Then start playit.gg

5. **Check if speed improved**

---

### Fix 3: Recreate Tunnel

**New tunnel might get better server:**

1. **Go to playit.gg dashboard:**
   - https://playit.gg/account/agents

2. **Delete old tunnel:**
   - Click on "Minecraft Java" tunnel
   - Delete/Remove it

3. **Create new tunnel:**
   - Click "Add Tunnel"
   - Select "Minecraft Java"
   - Local address: `127.0.0.1:25565`
   - **Choose region:** Singapore1 or Tokyo1
   - Save

4. **Get new connection info:**
   - Note new IP:PORT
   - Share with friends

---

### Fix 4: Check playit.gg Dashboard

**See if there are limits:**

1. **Go to dashboard:**
   - https://playit.gg/account/agents

2. **Check:**
   - Usage charts (if available)
   - Any warnings about limits
   - Tunnel status (should be "Active")

3. **If you see bandwidth limits:**
   - Free tier has limits
   - Consider upgrading or using alternative

---

### Fix 5: Try Different Time

**Server load varies by time:**

- **Peak hours** (evening): Slower
- **Off-peak hours** (morning): Faster
- Try playing at different times

---

## Alternative: Use Tunnely

**If playit.gg keeps being slow:**

- **Tunnely** is often faster for Minecraft
- See: `TUNNELY-SETUP.md`
- Made specifically for game servers
- Usually more stable

---

## Step-by-Step Troubleshooting

**Try this exact order:**

1. **Change tunnel region to Singapore1:**
   - Dashboard → Tunnel → Region → Singapore1
   - Wait for reconnect

2. **Test speed:**
   - Connect and see if it's better

3. **If still slow, restart everything:**
   - Stop server (Ctrl+C)
   - Close playit.gg
   - Wait 30 seconds
   - Start server
   - Start playit.gg

4. **If still slow, recreate tunnel:**
   - Delete old tunnel
   - Create new one with Singapore1 region

5. **If still slow:**
   - Try Tunnely instead
   - Or use local network (192.168.1.6:25565) if friends are on same WiFi

---

## Summary

**To fix playit.gg slowness:**

1. ✅ Change region to **Singapore1** (closest)
2. ✅ Restart server and playit.gg
3. ✅ Recreate tunnel with better region
4. ✅ Check dashboard for limits
5. ✅ Consider Tunnely if still slow

**Most common fix: Change region to Singapore1!**







