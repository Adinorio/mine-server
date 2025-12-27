# PLDT Router - Finding Virtual Server (Port Forwarding)

## You're Currently On
**Network → Wlan Settings → WPS**

This is the **wrong section** - WPS is for Wi-Fi setup, not port forwarding!

---

## Where to Find Port Forwarding

### Look in the Left Sidebar

Scroll through the **left sidebar** and look for:

1. **"NAT"** or **"NAT Settings"**
   - Click this
   - Then look for **"Virtual Server"** inside

2. **"Virtual Server"** (direct link)
   - Might be directly in the sidebar
   - Not under Wlan Settings

3. **"Advanced"** (under Network)
   - Click this
   - Look for **"Virtual Server"** inside

4. **"Port Forwarding"** (some routers)
   - Direct link in sidebar

---

## Common PLDT Router Structure

```
Network
├── Wlan Settings (you're here - wrong section)
│   ├── Band Steering
│   ├── Basic
│   ├── Advanced
│   ├── 5G Basic
│   ├── 5G Advanced
│   └── WPS ← You're here
│
├── NAT (look here!) ← Port forwarding is usually here
│   └── Virtual Server ← This is what you need!
│
├── Advanced (or check here)
│   └── Virtual Server
│
└── Virtual Server (might be direct link)
```

---

## Step-by-Step

### Step 1: Look at Left Sidebar
- Scroll up/down in the **left sidebar**
- Look for menu items **outside** of "Wlan Settings"
- Common names: **"NAT"**, **"Virtual Server"**, **"Advanced"**

### Step 2: Click "NAT" or "Virtual Server"
- Most PLDT routers have it under **"NAT"**
- Click it

### Step 3: Find "Virtual Server"
- Inside NAT section, look for **"Virtual Server"**
- Click it

### Step 4: Add Port Forwarding Rule
Once you find Virtual Server, add:

| Field | Value |
|-------|-------|
| **Service Name** | `Minecraft Server` |
| **External Port** | `25565` |
| **Internal Port** | `25565` |
| **Protocol** | `TCP` |
| **Internal IP** | `192.168.1.6` |
| **Status** | `Enabled` |

---

## If You Can't Find It

### Check These Places:
1. **Scroll down** in left sidebar - might be below
2. **Look for "NAT"** - most common location
3. **Check "Advanced"** section
4. **Some PLDT routers** have it under **"Firewall"**

### Alternative Names:
- "Virtual Server"
- "Port Mapping"
- "NAT Forwarding"
- "Port Forwarding"

---

## Quick Action

**Right now:**
1. Look at the **left sidebar** (where you see "Wlan Settings")
2. Scroll and look for **"NAT"** or **"Virtual Server"**
3. Click it
4. Add your port forwarding rule

**The port forwarding is NOT under "Wlan Settings" - it's a separate section!**

---

## What You Should See

When you find Virtual Server, you'll see:
- A table/list of port forwarding rules
- An "Add" or "Add New" button
- Fields for port numbers, IP address, protocol

That's where you add your Minecraft server rule!








