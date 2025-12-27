# Port Forwarding Guide for Minecraft Server

## What is Port Forwarding?
Port forwarding allows external connections to reach your Minecraft server through your router.

## Requirements
- Access to your router's admin panel
- Your laptop's local IP address (usually 192.168.x.x)
- Port 25565 (Minecraft default)

## Step-by-Step Instructions

### Step 1: Find Your Router's Admin Panel
1. Open Command Prompt or PowerShell
2. Run: `ipconfig`
3. Find "Default Gateway" (usually `192.168.1.1` or `192.168.0.1`)
4. Open this IP in your web browser

### Step 2: Log In to Router
- Default username/password is often on a sticker on your router
- Common defaults: `admin/admin` or `admin/password`
- If changed, ask whoever set up your WiFi

### Step 3: Find Port Forwarding Section
Look for:
- "Port Forwarding"
- "Virtual Server"
- "NAT Forwarding"
- "Applications & Gaming"
- Usually under "Advanced" or "Network" settings

### Step 4: Add Port Forwarding Rule
Create a new rule with these settings:

**Service Name:** Minecraft Server
**External Port:** 25565
**Internal Port:** 25565
**Protocol:** TCP (or Both/All)
**Internal IP:** Your laptop's IP (e.g., `192.168.1.8`)
**Status:** Enabled

### Step 5: Find Your Public IP
1. Visit: https://www.whatismyip.com/
2. Copy your public IP address (e.g., `123.45.67.89`)

### Step 6: Share with Friends
- **Address:** `YOUR_PUBLIC_IP:25565`
- Example: `123.45.67.89:25565`

## Security Tips
1. **Enable Whitelist** in `server.properties`:
   ```
   white-list=true
   ```
   Then add players: `/whitelist add PlayerName`

2. **Use Strong Passwords** if using server plugins

3. **Keep Server Updated** for security patches

4. **Only Share IP with Trusted Friends**

## Troubleshooting

### Friends Can't Connect?
1. **Check Firewall:** Allow Java through Windows Firewall
2. **Verify Port Forwarding:** Make sure rule is enabled
3. **Check Public IP:** It might have changed (use Dynamic DNS if needed)
4. **Test Locally First:** Make sure `192.168.1.8:25565` works

### Public IP Changed?
- Your ISP may assign a new IP when you restart your router
- Use Dynamic DNS (see Option 3) for a permanent address

## Find Your Local IP Anytime
Run: `.\get-my-ip.ps1`









