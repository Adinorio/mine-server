# Port Forwarding - Step by Step Guide

## Your Information
- **Router:** `192.168.1.1` (or check with `ipconfig`)
- **Your Laptop IP:** `192.168.1.8`
- **Port:** `25565`
- **Protocol:** `TCP`

---

## Step 1: Access Router Admin Panel

### Method 1: Automatic
1. Open your web browser
2. Go to: `http://192.168.1.1`
3. If that doesn't work, try: `http://192.168.0.1`

### Method 2: Find Your Router IP
1. Open PowerShell
2. Run: `ipconfig`
3. Look for "Default Gateway"
4. Open that IP in your browser

---

## Step 2: Log In to Router

### Find Login Credentials:
1. **Check router sticker** - username/password often printed there
2. **Common defaults:**
   - Username: `admin` / Password: `admin`
   - Username: `admin` / Password: `password`
   - Username: `admin` / Password: (blank/empty)
   - Username: `admin` / Password: `1234`

3. **If changed:** Ask whoever set up the WiFi

---

## Step 3: Find Port Forwarding Section

The location varies by router brand. Look for:

### Common Menu Names:
- **"Port Forwarding"**
- **"Virtual Server"**
- **"NAT Forwarding"**
- **"Applications & Gaming"**
- **"Firewall" → "Port Forwarding"**
- **"Advanced" → "Port Forwarding"**

### Common Locations by Brand:

**TP-Link:**
- Advanced → NAT Forwarding → Port Forwarding

**Netgear:**
- Advanced → Port Forwarding / Port Triggering

**Linksys:**
- Smart Wi-Fi Tools → Port Forwarding

**ASUS:**
- WAN → Virtual Server / Port Forwarding

**D-Link:**
- Advanced → Port Forwarding

**If you can't find it:**
- Search online: "[Your Router Brand Model] port forwarding"
- Check router manual

---

## Step 4: Add Port Forwarding Rule

Click **"Add"**, **"Create New Rule"**, or **"Add Custom Service"**

### Enter These Settings:

| Field | Value |
|-------|-------|
| **Service Name** | `Minecraft Server` |
| **External Port** | `25565` |
| **Internal Port** | `25565` |
| **Protocol** | `TCP` (or `Both`/`All`) |
| **Internal IP** | `192.168.1.8` (your laptop's IP) |
| **Status** | `Enabled` |

**Important:** 
- Make sure "Internal IP" matches your laptop's IP (`192.168.1.8`)
- Both External and Internal ports should be `25565`
- Protocol should be `TCP` (or `Both` if TCP isn't available)

---

## Step 5: Save and Apply

1. Click **"Save"** or **"Apply"**
2. Router may restart (wait 1-2 minutes)
3. Wait for router to come back online

---

## Step 6: Find Your Public IP

1. Visit: https://www.whatismyip.com/
2. Copy your public IP address
3. It will look like: `123.45.67.89`

**Note:** This IP might change if you restart your router. For a permanent address, use Dynamic DNS (see below).

---

## Step 7: Test Connection

1. **Make sure server is running:**
   ```powershell
   .\start-server.ps1
   ```

2. **Test locally first:**
   - Connect to: `localhost` or `192.168.1.8:25565`
   - Should work!

3. **Test from another device:**
   - Use your public IP: `YOUR_PUBLIC_IP:25565`
   - Should work if port forwarding is correct!

---

## Step 8: Share with Friends

Give them:
- **Address:** `YOUR_PUBLIC_IP:25565`
- **Version:** 1.21.11

Example: `123.45.67.89:25565`

---

## Troubleshooting

### Can't Access Router?
- Make sure you're on the same network
- Try `http://` instead of `https://`
- Check router manual for default IP

### Can't Find Port Forwarding?
- Search online: "[Your Router Brand] port forwarding"
- Check router manual
- Some routers call it "Virtual Server" or "NAT Forwarding"

### Friends Still Can't Connect?
1. **Check Windows Firewall:**
   - Run: `.\fix-firewall.ps1` as Administrator
   - Or manually allow Java through firewall

2. **Verify port forwarding:**
   - Make sure rule is enabled
   - Check Internal IP is correct (`192.168.1.8`)
   - Check ports are `25565`

3. **Test locally first:**
   - `192.168.1.8:25565` should work
   - If local works but external doesn't, port forwarding issue

4. **Check public IP:**
   - It might have changed
   - Get new IP from: https://www.whatismyip.com/

### Public IP Changed?
- Your ISP may assign a new IP when router restarts
- Use Dynamic DNS for permanent address (see below)

---

## Dynamic DNS (Optional - For Permanent Address)

If your public IP changes frequently:

1. **Sign up for free Dynamic DNS:**
   - No-IP: https://www.noip.com/ (free)
   - DuckDNS: https://www.duckdns.org/ (free)

2. **Install their client** on your laptop

3. **Get a domain** (e.g., `myserver.ddns.net`)

4. **Share:** `myserver.ddns.net:25565` (never changes!)

---

## Quick Reference

- **Router:** `http://192.168.1.1`
- **Your IP:** `192.168.1.8`
- **Port:** `25565`
- **Protocol:** `TCP`
- **Public IP:** Get from https://www.whatismyip.com/

---

## Need Help?

- **Find router IP:** Run `ipconfig` and look for "Default Gateway"
- **Find your IP:** Run `.\get-my-ip.ps1`
- **Fix firewall:** Run `.\fix-firewall.ps1` as Administrator
- **Router-specific help:** Search "[Your Router Brand] port forwarding"
