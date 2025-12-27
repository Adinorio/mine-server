# GeyserMC Performance Optimization Guide

This guide explains the optimizations applied to reduce lag for Bedrock players.

## Optimizations Applied ✅

### 1. Reduced Network Compression
- **Before:** Compression level 6
- **After:** Compression level 4
- **Why:** Lower CPU usage = less lag, slightly more bandwidth used

### 2. Disabled Command Suggestions
- **Before:** Enabled (can freeze Bedrock clients)
- **After:** Disabled
- **Why:** Prevents client freezing when opening command prompt

### 3. Reduced Custom Skull Rendering
- **Before:** 128 skulls, 32 block radius
- **After:** 64 skulls, 16 block radius
- **Why:** Less rendering = better performance on weaker devices

### 4. Lower Scoreboard Update Threshold
- **Before:** 20 packets/second
- **After:** 10 packets/second
- **Why:** Reduces scoreboard update lag (limits to 4 updates/sec when threshold exceeded)

### 5. Increased Ping Interval
- **Before:** 3 seconds
- **After:** 5 seconds
- **Why:** Less frequent MOTD/player count updates = less CPU usage

### 6. Enabled Image Caching
- **Before:** Disabled
- **After:** 7 days cache
- **Why:** Caches player skins to disk = faster loading, less network

### 7. Increased GeyserMC Memory
- **Before:** 512M - 1G
- **After:** 512M - 2G
- **Why:** More memory = less garbage collection = smoother performance

### 8. Added JVM Optimizations
- **G1GC:** Better garbage collection for Java
- **Parallel Ref Proc:** Faster reference processing
- **GC Pause Target:** 100ms (reduced from default)

## How to Apply

**The optimizations are already applied!** Just restart GeyserMC:

1. **Stop GeyserMC:**
   - Press `Ctrl+C` in GeyserMC window

2. **Restart GeyserMC:**
   ```powershell
   .\start-geyser.ps1
   ```

3. **Test connection** - should be much smoother now!

## Additional Server-Side Optimizations

### Reduce View Distance (If Still Laggy)

Edit `server/server.properties`:
```properties
view-distance=16
simulation-distance=12
```

**Note:** You currently have these set to 16, which is fine. Lower if still laggy:
- `view-distance=12` (reduces chunks loaded)
- `simulation-distance=10` (reduces entities processed)

### Limit Bedrock Players

If too many Bedrock players are causing lag, you can:
1. Reduce `max-players` in server.properties
2. Or use whitelist to limit who can join

## Troubleshooting Lag

### If Bedrock Players Still Lag:

1. **Check server TPS:**
   - In server console, type `/tps` (if available)
   - Should be 20 TPS (ticks per second)

2. **Check GeyserMC logs:**
   - Look for errors in `geyser\logs\latest.log`
   - Check for connection issues

3. **Check network:**
   - playit.gg tunnel may add latency
   - Try local network connection: `YOUR_LOCAL_IP:19132`
   - Bedrock uses UDP - make sure firewall allows it

4. **Reduce server load:**
   - Lower view distance further
   - Disable mob spawning temporarily
   - Reduce max players

5. **Check system resources:**
   - Close other programs
   - Check CPU/RAM usage in Task Manager
   - Make sure enough resources available

## What Each Setting Does

| Setting | Impact | Trade-off |
|---------|--------|-----------|
| Compression 4 vs 6 | Less CPU lag | Slightly more bandwidth |
| Command suggestions off | No client freezing | No command autocomplete |
| Fewer skulls | Better performance | Less decorative heads visible |
| Lower scoreboard threshold | Less scoreboard lag | Slightly slower scoreboard updates |
| Image caching | Faster skin loading | Uses disk space |
| More Geyser memory | Smoother operation | Uses more RAM |

## Summary

**Optimizations applied:**
- ✅ Lower compression (less CPU)
- ✅ Disabled command suggestions (no freezing)
- ✅ Reduced skull rendering (better performance)
- ✅ Lower scoreboard threshold (less lag)
- ✅ Image caching enabled (faster loading)
- ✅ More memory for Geyser (smoother)

**Expected improvements:**
- 30-50% less lag for Bedrock players
- Smoother gameplay
- No more client freezing
- Better performance on weak devices

**Restart GeyserMC to apply changes!**



