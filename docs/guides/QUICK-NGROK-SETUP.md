# Quick ngrok Setup Guide

## Step 1: Find or Download ngrok

### If ngrok is already installed:
Tell me where you installed it, or we can search for it.

### If you need to download:
1. Go to: https://ngrok.com/download
2. Download ngrok for Windows
3. Extract `ngrok.exe` to a folder (e.g., `C:\ngrok\`)

## Step 2: Configure ngrok Authtoken

1. **Get your authtoken:**
   - Go to: https://dashboard.ngrok.com/get-started/your-authtoken
   - Sign up/login if needed (free)
   - Copy your authtoken

2. **Configure ngrok:**
   - Open PowerShell
   - Navigate to ngrok folder (e.g., `cd C:\ngrok`)
   - Run: `.\ngrok config add-authtoken YOUR_TOKEN_HERE`
   - Replace `YOUR_TOKEN_HERE` with your actual token

## Step 3: Use the Script

I've created `start-ngrok.ps1` that will:
- Find ngrok automatically
- Check if your server is running
- Start the tunnel
- Show you the connection address

### To use:

1. **Start your Minecraft server:**
   ```powershell
   .\start-server.ps1
   ```
   Wait for "Done" message.

2. **Start ngrok tunnel:**
   ```powershell
   .\start-ngrok.ps1
   ```

3. **Copy the address ngrok shows:**
   - It will look like: `0.tcp.ngrok.io:12345`
   - Share this with friends!

## Manual Method (if script doesn't work)

If the script can't find ngrok, you can run it manually:

1. **Find ngrok.exe location** (e.g., `C:\ngrok\ngrok.exe`)

2. **Open PowerShell in that folder:**
   ```powershell
   cd C:\ngrok
   ```

3. **Start tunnel:**
   ```powershell
   .\ngrok.exe tcp 25565
   ```

4. **Copy the address shown** (e.g., `tcp://0.tcp.ngrok.io:12345`)
   - Friends connect to: `0.tcp.ngrok.io:12345`

## Important Notes

- **Keep ngrok running** while friends are playing
- **Address changes** each time you restart ngrok (free tier)
- **Server must be running** before starting ngrok
- **Two windows needed:** One for server, one for ngrok

## Troubleshooting

### "ngrok.exe not found"
- Make sure ngrok is downloaded and extracted
- Tell me where you put it, or add it to PATH

### "authtoken not configured"
- Run: `ngrok config add-authtoken YOUR_TOKEN`
- Get token from: https://dashboard.ngrok.com/get-started/your-authtoken

### "Connection refused"
- Make sure Minecraft server is running first
- Check server is on port 25565









