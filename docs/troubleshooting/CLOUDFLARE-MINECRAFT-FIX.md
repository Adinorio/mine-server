# Cloudflare Tunnel for Minecraft - Important Note

## The Issue

Cloudflare's quick tunnel (`--url tcp://localhost:25565`) creates an **HTTPS endpoint**, but Minecraft needs a **TCP connection**.

The URL shown (`watt-pays-memo-processed.trycloudflare.com`) is an HTTPS URL, not a TCP endpoint that Minecraft can connect to.

## Solutions

### Option 1: Use Port Forwarding (Recommended)

Since Cloudflare quick tunnel doesn't work well for Minecraft TCP, use **port forwarding** instead:

1. See: `PORT-FORWARDING-GUIDE.md`
2. Forward port 25565 on your router
3. Share your public IP: `YOUR_PUBLIC_IP:25565`

**This is the most reliable method for Minecraft!**

---

### Option 2: Use Local Network (Works Now!)

For friends on the same WiFi:
- **Address:** `192.168.1.8:25565`
- **No setup needed** - already works!

---

### Option 3: Cloudflare Named Tunnel (Advanced)

For a permanent Cloudflare tunnel with TCP support:

1. **Sign up for Cloudflare account** (free)
2. **Create a named tunnel:**
   ```powershell
   .\cloudflared.exe tunnel login
   .\cloudflared.exe tunnel create minecraft
   ```
3. **Configure for TCP** (requires domain setup)
4. See: `CLOUDFLARE-TUNNEL-SETUP.md` for full guide

**Note:** This requires more setup but gives permanent TCP endpoint.

---

## Why Quick Tunnel Doesn't Work

Cloudflare's quick tunnel (`trycloudflare.com`) is designed for **HTTP/HTTPS traffic**, not raw TCP connections that Minecraft uses.

For Minecraft, you need:
- **Port forwarding** (best option)
- **Local network** (for same WiFi)
- **Named Cloudflare tunnel** (advanced, requires domain)

---

## Recommendation

**Use Port Forwarding** - it's free, permanent, and works perfectly for Minecraft!

See: `PORT-FORWARDING-GUIDE.md` for step-by-step instructions.









