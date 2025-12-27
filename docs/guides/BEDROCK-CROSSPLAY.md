# Bedrock Crossplay Setup Guide

This guide explains how to enable Bedrock Edition (PE/mobile/Xbox/PlayStation) players to join your Java Edition server using GeyserMC.

## How It Works

GeyserMC acts as a **protocol translator** between Bedrock and Java editions:
- Bedrock players connect to Geyser on port **19132**
- Geyser translates their connection and forwards it to your Java server on port **25565**
- Java players continue connecting directly to port **25565** (no changes needed)

## Setup Steps

### 1. Install GeyserMC

Run the setup script:

```powershell
.\setup-geyser.ps1
```

This will:
- Download GeyserMC standalone proxy
- Create configuration files
- Set up the connection to your Java server

### 2. Configure Geyser

After running setup, edit `geyser\config.yml` and ensure these settings:

```yaml
bedrock:
  address: 0.0.0.0
  port: 19132

remote:
  address: 127.0.0.1
  port: 25565
  auth-type: offline
```

**Important settings:**
- `remote.address: 127.0.0.1` - Your Java server (same computer)
- `remote.port: 25565` - Your Java server port
- `remote.auth-type: offline` - Since your server has `online-mode=false`
- `bedrock.port: 19132` - Port Bedrock players will use

### 3. Start Your Server and Geyser

**Important:** You need BOTH running at the same time!

1. **Start your Java server:**
   ```powershell
   .\start-server.ps1
   ```

2. **Start Geyser proxy** (in a separate terminal/window):
   ```powershell
   .\start-geyser.ps1
   ```

Keep both windows open while players are connected.

## Connection Instructions

### For Bedrock Players (PE/Mobile/Xbox/PlayStation/Switch)

Bedrock players need to connect using a **different port** than Java players:

#### On Local Network (Same WiFi):
- **Address:** `YOUR_LOCAL_IP:19132`
  - Example: `192.168.1.100:19132`
  - Your local IP is shown when you start Geyser

#### From Internet (Online):
- **If using playit.gg:** Use the Bedrock tunnel address from playit.gg dashboard
  - Example: `xyz789.gl.joinmc.link:45678` (your Bedrock tunnel address)
- **If using port forwarding:** `YOUR_PUBLIC_IP:19132`
- **Or other tunnels:** `YOUR_TUNNEL_URL:19132`

#### Steps for Bedrock Players:

1. **On Mobile (PE):**
   - Open Minecraft PE
   - Tap "Play"
   - Tap "Servers" tab
   - Scroll down and tap "Add Server"
   - Enter server name (e.g., "My Server")
   - Enter server address: `IP:19132`
   - Tap "Save" and join

2. **On Xbox/PlayStation/Switch:**
   - Go to "Servers" section
   - Add server with address: `IP:19132`
   - Join server

### For Java Players

Java players continue connecting as normal:
- **Local:** `192.168.x.x:25565`
- **Online:** `YOUR_IP:25565` or tunnel URL

**No changes needed for Java players!**

## Using playit.gg for Bedrock Players (Recommended)

If you're already using **playit.gg** for Java players, you can add a second tunnel for Bedrock!

### Setup playit.gg for Bedrock

1. **Open playit.gg dashboard:** https://playit.gg/account/agents
2. **Add a new tunnel:**
   - Click "Create Tunnel" or "+"
   - **Tunnel Type:** Select "Minecraft Bedrock" or "UDP"
   - **Local Port:** `19132`
   - **Local Address:** `127.0.0.1` or `localhost`
3. **Save the tunnel** - playit.gg will give you a new address/port
4. **Note the connection info:**
   - You'll get something like: `abc123.playit.gg:45678`
   - Or IP: `147.185.221.24:45678`

### Connection Addresses

Now you'll have **two playit.gg addresses:**

- **Java players:** Your existing tunnel (port 25565)
  - Example: `run-generous.gl.joinmc.link:37795`
- **Bedrock players:** New Bedrock tunnel (port 19132)
  - Example: `xyz789.gl.joinmc.link:45678`

Both tunnels work at the same time! Java and Bedrock players can play together.

## Port Forwarding for Bedrock (Alternative)

If you prefer port forwarding instead of playit.gg, forward **port 19132** (UDP protocol):

1. Open your router admin panel
2. Go to Port Forwarding
3. Add rule:
   - **Service Name:** Minecraft Bedrock
   - **External Port:** 19132
   - **Internal Port:** 19132
   - **Protocol:** UDP (or Both)
   - **Internal IP:** Your laptop's IP

**Note:** You may already have port 25565 forwarded for Java players - Bedrock needs port 19132 separately!

## Troubleshooting

### Bedrock Players Can't Connect

1. **Check all three are running:**
   - ✅ Java server must be running on port 25565
   - ✅ Geyser must be running on port 19132
   - ✅ playit.gg must be running (if using playit.gg)

2. **If using playit.gg:**
   - Check playit.gg dashboard has a tunnel for port 19132
   - Make sure Bedrock tunnel is active/connected
   - Use the correct Bedrock tunnel address (different from Java tunnel!)
   - Bedrock tunnel should use UDP protocol

3. **Check firewall:**
   - Allow port 19132 (UDP) through Windows Firewall
   - Run: `.\FIX-FIREWALL-BEDROCK.bat` (opens UDP port 19132)

4. **If using port forwarding:**
   - Ensure port 19132 UDP is forwarded in router
   - Java uses TCP (25565), Bedrock uses UDP (19132) - different protocols!

5. **Check Geyser config:**
   - Ensure `remote.address` is `127.0.0.1` in `geyser\config.yml`
   - Ensure `remote.port` is `25565`
   - Ensure `bedrock.port` is `19132`

### Geyser Won't Start

1. **Check Java:**
   - Geyser needs Java 16+ (same as your server)
   - Run `java -version` to check

2. **Check port 19132:**
   - Make sure nothing else is using port 19132
   - Try changing port in `geyser\config.yml` if needed

### Players Can't See Each Other

- **Java and Bedrock players can play together!** This is normal.
- Some features may differ slightly (e.g., shields, some blocks)
- Most gameplay is compatible

## Important Notes

- **Server must run first:** Start Java server before Geyser
- **Keep both running:** Both must be active for crossplay to work
- **Different ports:**
  - Java: `25565` (TCP)
  - Bedrock: `19132` (UDP)
- **Online mode:** Your server has `online-mode=false`, so Geyser uses `auth-type: offline`
- **Performance:** Geyser adds minimal overhead, but monitor server performance

## Stopping

1. Stop Geyser: Press `Ctrl+C` in Geyser window
2. Stop Server: Press `Ctrl+C` in server window (or run `.\stop-server.ps1`)

## Advanced Configuration

Edit `geyser\config.yml` to customize:
- Change Bedrock port (default: 19132)
- Adjust performance settings
- Enable/disable features
- Configure Floodgate (advanced authentication)

For more details, see: https://geysermc.org/docs/

