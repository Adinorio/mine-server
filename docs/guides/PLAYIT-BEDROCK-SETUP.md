# playit.gg Setup for Bedrock Crossplay

This guide shows you how to configure playit.gg to work with both Java and Bedrock players.

## How It Works

playit.gg can tunnel **multiple ports** at the same time:
- **Java tunnel** (TCP port 25565) - for Java Edition players
- **Bedrock tunnel** (UDP port 19132) - for Bedrock Edition players via GeyserMC

Both tunnels run simultaneously, allowing Java and Bedrock players to play together!

## Setup Steps

### Step 1: Set Up GeyserMC

First, install and configure GeyserMC:

```powershell
.\setup-geyser.ps1
```

This creates GeyserMC proxy that listens on port 19132 for Bedrock players.

### Step 2: Configure playit.gg

1. **Open playit.gg dashboard:**
   - Go to: https://playit.gg/account/agents
   - Log in if needed

2. **Check your existing Java tunnel:**
   - You should already have a tunnel for "Minecraft Java"
   - This tunnels port 25565 (TCP)
   - Keep this running - Java players use it

3. **Add Bedrock tunnel:**
   - Click **"Create Tunnel"** or **"+"** button
   - **Select tunnel type:**
     - Look for "Minecraft Bedrock" option
     - Or select "UDP" protocol
   - **Local address:** `127.0.0.1` (or `localhost`)
   - **Local port:** `19132`
   - **Name:** "Minecraft Bedrock" (optional, for easy identification)
   - Click **"Create"** or **"Save"**

4. **Get Bedrock connection address:**
   - playit.gg will show you a connection address
   - Format: `domain.gl.joinmc.link:PORT` or `IP:PORT`
   - **Write this down** - Bedrock players will use this!

### Step 3: Start Everything

You need **three things** running:

1. **Java Server:**
   ```powershell
   .\start-server.ps1
   ```

2. **GeyserMC Proxy:**
   ```powershell
   .\start-geyser.ps1
   ```

3. **playit.gg:**
   - Keep playit.gg application running
   - Both tunnels should show as "active" or "connected"

## Connection Addresses

After setup, you'll have **two different addresses:**

### Java Players
- Use your **existing Java tunnel** address
- Example: `run-generous.gl.joinmc.link:37795`
- Or: `147.185.221.24:37795`
- Port: Whatever port playit.gg assigned (usually different from 25565)

### Bedrock Players
- Use your **new Bedrock tunnel** address
- Example: `xyz789.gl.joinmc.link:45678`
- Or: `147.185.221.24:45678`
- Port: Different from Java port (assigned by playit.gg)

**Important:** Bedrock players use a **different address** than Java players!

## Sharing with Friends

### For Java Players (TLauncher/Java Edition)
```
run-generous.gl.joinmc.link:37795
```
*(Use your actual Java tunnel address)*

### For Bedrock Players (PE/Mobile/Xbox/PlayStation/Switch)
```
xyz789.gl.joinmc.link:45678
```
*(Use your actual Bedrock tunnel address)*

## Visual Setup Diagram

```
Internet Players
       │
       ├─ Java Player → playit.gg (Java tunnel) → localhost:25565 → Java Server
       │
       └─ Bedrock Player → playit.gg (Bedrock tunnel) → localhost:19132 → GeyserMC → localhost:25565 → Java Server
                                                                                              │
                                                                                              └─ Both connect to same server!
```

## Troubleshooting

### Both Tunnels Not Showing in playit.gg

- **Solution:** Make sure you created the Bedrock tunnel correctly
- Check tunnel type is "UDP" or "Minecraft Bedrock"
- Ensure local port is `19132`
- Restart playit.gg if needed

### Bedrock Players Can't Connect

1. **Check Geyser is running:**
   - Must run `.\start-geyser.ps1`
   - Geyser listens on port 19132

2. **Check Bedrock tunnel in playit.gg:**
   - Should show as "active" or "connected"
   - Note the correct address/port for Bedrock players

3. **Check firewall:**
   - Run `.\FIX-FIREWALL-BEDROCK.bat` to open UDP port 19132

4. **Verify addresses:**
   - Java players use Java tunnel address
   - Bedrock players use Bedrock tunnel address
   - They're different!

### Java Players Still Work Fine

- Your existing Java tunnel continues working
- No changes needed for Java players
- They use the same address as before

## Quick Reference

**To start everything:**
```powershell
# Terminal 1: Java Server
.\start-server.ps1

# Terminal 2: GeyserMC
.\start-geyser.ps1

# Terminal 3 (or keep running): playit.gg
# (Keep playit.gg application open)
```

**Connection addresses:**
- **Java:** Check playit.gg dashboard → Java tunnel → Copy address
- **Bedrock:** Check playit.gg dashboard → Bedrock tunnel → Copy address

**Both must be different addresses!**

## Notes

- Both tunnels work at the same time
- playit.gg supports multiple tunnels per agent
- Addresses may change if you restart playit.gg
- Always check dashboard after restarting playit.gg
- Java uses TCP, Bedrock uses UDP (different protocols)



