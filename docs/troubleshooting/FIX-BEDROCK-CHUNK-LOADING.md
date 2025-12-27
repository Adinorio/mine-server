# Fix Bedrock Chunk Loading Issues

This guide fixes chunks not loading for Bedrock players (especially on phones).

## Changes Applied ✅

### 1. Reduced Server View Distance
- **Before:** 16 chunks
- **After:** 12 chunks
- **Why:** Phones struggle with high view distances, 12 is more manageable

### 2. Reduced Simulation Distance
- **Before:** 16 chunks
- **After:** 10 chunks
- **Why:** Less processing = better performance, chunks still load fine

### 3. Lowered MTU (Maximum Transmission Unit)
- **Before:** 1400 bytes
- **After:** 1200 bytes
- **Why:** Prevents packet fragmentation on mobile networks, helps chunks load

## Apply Changes

**Restart both server and GeyserMC:**

1. **Stop Java Server:**
   - Press `Ctrl+C` in server window

2. **Stop GeyserMC:**
   - Press `Ctrl+C` in GeyserMC window

3. **Restart Server:**
   ```powershell
   .\start-server.ps1
   ```

4. **Restart GeyserMC:**
   ```powershell
   .\start-geyser.ps1
   ```

5. **Test connection** from phone

## Additional Fixes for Bedrock Players

### Tell Bedrock Players to Adjust Settings:

**On Minecraft PE (Phone):**

1. **Open Settings** (gear icon)
2. **Go to Video settings**
3. **Set Render Distance:**
   - **12 chunks** (matches server)
   - Or try **10 chunks** if still having issues
4. **Disable "Beautiful Skies"** (can cause issues)
5. **Reduce Graphics quality** if on low-end phone:
   - Set to "Fancy" or lower
6. **Save and restart game**

### Why This Fixes Chunk Loading:

1. **View Distance Match:**
   - Server: 12 chunks
   - Client: 12 chunks
   - Prevents mismatch that causes chunks not to load

2. **Lower MTU:**
   - Smaller packets = less likely to fragment
   - Mobile networks handle smaller packets better
   - Chunks load more reliably

3. **Reduced Server Load:**
   - Less chunks to process = faster chunk sending
   - Better performance for all players

## If Chunks Still Don't Load

### For Bedrock Players:

1. **Check Internet Connection:**
   - Make sure WiFi or mobile data is stable
   - Try switching networks (WiFi ↔ Mobile Data)

2. **Reconnect to Server:**
   - Disconnect and reconnect
   - Sometimes helps reset chunk loading

3. **Try Different Render Distance:**
   - Try 10 chunks first
   - If works, slowly increase to 12

4. **Check Phone Storage:**
   - Low storage can cause chunk loading issues
   - Free up space if needed

### Server-Side Checks:

1. **Check GeyserMC Logs:**
   - Look in `geyser\logs\latest.log`
   - Check for errors about chunk sending

2. **Monitor Server Performance:**
   - Type `/tps` in server console (if available)
   - Should be 20 TPS (ticks per second)
   - Low TPS = chunks load slowly

3. **Check playit.gg Connection:**
   - Bedrock tunnel might have high latency
   - Try local network: `YOUR_LOCAL_IP:19132` (if on same WiFi)

## Optimized Settings Summary

**Server:**
- `view-distance=12` (was 16)
- `simulation-distance=10` (was 16)

**GeyserMC:**
- `mtu=1200` (was 1400)
- `compression-level=4` (already optimized)

**Bedrock Client (Player's Phone):**
- Render Distance: **12 chunks**
- Graphics: Fancy or lower
- Beautiful Skies: **Disabled**

## Expected Results

After applying these fixes:
- ✅ Chunks should load properly on phones
- ✅ Smoother gameplay
- ✅ Less lag and stuttering
- ✅ Better connection stability

**Restart server and GeyserMC to apply changes!**



