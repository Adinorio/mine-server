# Fixing Server Lag and Timeout Issues

## The Problem

**Symptoms:**
- "Can't keep up! Is the server overloaded? Running 2064ms or 41 ticks behind"
- Players disconnect with "Timed out"
- Server freezes or stutters

**Causes:**
1. **Not enough RAM** - Server needs more memory
2. **Too high view distance** - Loading too many chunks
3. **Poor JVM settings** - Not optimized for Minecraft
4. **playit.gg latency** - Tunnel adds delay

---

## Fixes Applied

### Fix 1: Increased Memory Allocation ✅

**Changed:**
- **Before:** 2G - 4G RAM
- **After:** 4G - 6G RAM

**Why:** More RAM = less lag, fewer timeouts

---

### Fix 2: Optimized JVM Settings ✅

**Added performance flags:**
- G1GC garbage collector (better for Minecraft)
- Optimized GC settings
- Aikar's flags (Minecraft server optimization)

**Why:** Better memory management = smoother gameplay

---

### Fix 3: Reduced View Distance ✅

**Changed:**
- **view-distance:** 10 → 8
- **simulation-distance:** 10 → 8

**Why:** Less chunks to load = less lag

---

## How to Apply Fixes

**The fixes are already applied! Just restart your server:**

1. **Stop current server:**
   - Press `Ctrl+C` in server window

2. **Start server again:**
   ```powershell
   .\start-server.ps1
   ```

3. **Wait for "Done!" message**

4. **Test connection again**

---

## Additional Optimizations (If Still Laggy)

### Option 1: Reduce View Distance More

**Edit `server/server.properties`:**
```
view-distance=6
simulation-distance=6
```

**Restart server**

---

### Option 2: Limit Players

**Edit `server/server.properties`:**
```
max-players=5
```

**Why:** Fewer players = less load

---

### Option 3: Disable Mob Spawning (Temporary)

**Edit `server/server.properties`:**
```
spawn-monsters=false
spawn-animals=false
```

**Why:** Less entities = better performance

---

## playit.gg Latency Issues

**If lag is from playit.gg tunnel:**

1. **Change region to Singapore1:**
   - Dashboard → Tunnel → Region → Singapore1

2. **Check playit.gg dashboard:**
   - Look for usage limits
   - Free tier may throttle

3. **Try local network:**
   - Use `192.168.1.6:25565` if friends are on same WiFi
   - Much faster than tunnel

---

## Monitoring Server Performance

**Watch server console for:**
- ✅ "Done!" = Server ready
- ⚠️ "Can't keep up!" = Still lagging
- ✅ No warnings = Good performance

**If you see "Can't keep up!" again:**
- Reduce view-distance more
- Check if other programs are using CPU/RAM
- Close unnecessary programs

---

## Summary

**Fixes applied:**
1. ✅ Increased RAM: 2G-4G → 4G-6G
2. ✅ Optimized JVM settings
3. ✅ Reduced view distance: 10 → 8

**Next steps:**
1. Restart server with `.\start-server.ps1`
2. Test connection
3. If still laggy, reduce view-distance to 6

**The server should now run much smoother!**





