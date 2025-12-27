# Cloudflare Tunnel Setup for Minecraft Server

## What is Cloudflare Tunnel?
Cloudflare Tunnel (cloudflared) creates a secure tunnel for your Minecraft server without needing port forwarding or a public IP.

## Step 1: Sign Up for Cloudflare (Free)

1. Go to: https://dash.cloudflare.com/sign-up
2. Sign up for a **free account** (no credit card needed)
3. Verify your email

## Step 2: Download cloudflared

1. Go to: https://developers.cloudflare.com/cloudflare-one/connections/connect-apps/install-and-setup/installation/
2. Download **cloudflared for Windows**
3. Extract `cloudflared.exe` to a folder (e.g., `C:\cloudflared\`)

**Or download directly:**
- Windows 64-bit: https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-windows-amd64.exe
- Rename to `cloudflared.exe`

## Step 3: Authenticate cloudflared

1. Open PowerShell
2. Navigate to cloudflared folder:
   ```powershell
   cd C:\cloudflared
   ```
3. Run:
   ```powershell
   .\cloudflared.exe tunnel login
   ```
4. This will open your browser - log in to Cloudflare
5. Select a domain (you can use a free subdomain or get a free domain)

## Step 4: Create a Tunnel

1. Create a tunnel:
   ```powershell
   .\cloudflared.exe tunnel create minecraft
   ```
   This creates a tunnel named "minecraft"

2. Note the tunnel ID that's shown (you'll need it)

## Step 5: Configure the Tunnel

Create a config file: `C:\cloudflared\config.yml`

```yaml
tunnel: YOUR_TUNNEL_ID
credentials-file: C:\Users\YOUR_USERNAME\.cloudflared\YOUR_TUNNEL_ID.json

ingress:
  - hostname: minecraft.yourdomain.com
    service: tcp://localhost:25565
  - service: http_status:404
```

**Replace:**
- `YOUR_TUNNEL_ID` with the tunnel ID from step 4
- `YOUR_USERNAME` with your Windows username
- `minecraft.yourdomain.com` with your domain/subdomain

## Step 6: Run the Tunnel

1. **Make sure your Minecraft server is running:**
   ```powershell
   .\start-server.ps1
   ```

2. **Start the tunnel:**
   ```powershell
   cd C:\cloudflared
   .\cloudflared.exe tunnel run minecraft
   ```

3. **The tunnel will show your domain** (e.g., `minecraft.yourdomain.com`)
4. **Share this with friends!**

## Alternative: Quick Tunnel (No Domain Needed)

If you don't want to set up a domain, use a quick tunnel:

```powershell
.\cloudflared.exe tunnel --url tcp://localhost:25565
```

This gives you a temporary URL like: `https://random-name.trycloudflare.com`

**Note:** Quick tunnel URLs change each time you restart.

## Important Notes

- **Keep cloudflared running** while friends are playing
- **Server must be running** before starting tunnel
- **Free tier:** Unlimited tunnels, no credit card needed
- **Permanent address:** If you use a domain (not quick tunnel)

## Troubleshooting

### "tunnel not found"
- Make sure you created the tunnel first
- Check tunnel ID is correct

### "Connection refused"
- Make sure Minecraft server is running on port 25565
- Check server is accessible: `localhost:25565`

### Domain not working?
- Make sure DNS is configured in Cloudflare dashboard
- Check tunnel is running

## Quick Start (Simplest Method)

If you just want to test quickly:

1. Download cloudflared.exe
2. Run:
   ```powershell
   .\cloudflared.exe tunnel --url tcp://localhost:25565
   ```
3. Share the URL shown (temporary, changes on restart)

For permanent address, follow the full setup above.









