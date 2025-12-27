# PLDT Home Fibr Router - Port Forwarding Guide

## Current Location
You're on: **Status → Wireless Status**

## Where to Find Port Forwarding

### Step 1: Go to Network Tab
1. Look at the **top navigation tabs**
2. Click on **"Network"** tab (next to "Status")
3. This will show network configuration options

### Step 2: Find Virtual Server or Port Forwarding
In the **Network** section, look for:
- **"Virtual Server"** (most common for PLDT)
- **"Port Forwarding"**
- **"NAT" → "Virtual Server"**
- **"Advanced" → "Virtual Server"**

### Common PLDT Router Locations:
- **Network → Virtual Server**
- **Network → NAT → Virtual Server**
- **Network → Advanced → Virtual Server**

---

## Step-by-Step for PLDT Router

### Step 1: Click "Network" Tab
- Top of the page, next to "Status"

### Step 2: Look in Left Sidebar
- Under "Network" section
- Find "Virtual Server" or "Port Forwarding"
- Click it

### Step 3: Add Rule
Click "Add" or "Add New Rule" and enter:

| Field | Value |
|-------|-------|
| **Service Name** | `Minecraft Server` |
| **External Port Start** | `25565` |
| **External Port End** | `25565` |
| **Internal Port Start** | `25565` |
| **Internal Port End** | `25565` |
| **Protocol** | `TCP` |
| **Internal IP** | `192.168.1.6` (your laptop IP) |
| **Status** | `Enabled` |

### Step 4: Save
- Click "Save" or "Apply"
- Wait for router to apply settings

---

## If You Can't Find It

### Alternative Names in PLDT Routers:
- "Virtual Server"
- "Port Mapping"
- "NAT Forwarding"
- "Port Triggering"

### Search in Router:
1. Look through all menu items in **Network** tab
2. Check **Advanced** section if available
3. Some PLDT routers have it under **"Firewall" → "Virtual Server"**

---

## Quick Navigation

**From where you are now:**
1. Click **"Network"** tab (top navigation)
2. Look for **"Virtual Server"** in left sidebar
3. Click it
4. Add your port forwarding rule

---

## Your Settings

- **Router:** PLDT Home Fibr
- **Your IP:** `192.168.1.6`
- **Port:** `25565`
- **Protocol:** `TCP`

---

## Need Help?

If you still can't find it:
- Check router manual
- Search: "PLDT Home Fibr port forwarding"
- Some PLDT routers may have it disabled - check with PLDT support








