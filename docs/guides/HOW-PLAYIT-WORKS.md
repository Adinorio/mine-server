# How playit.gg Makes Your Server Accessible (Non-Local)

This explains how playit.gg creates a tunnel that makes your local server accessible from anywhere on the internet.

## The Problem: Local vs Internet

**Your server is local:**
- Runs on your computer: `127.0.0.1:25565` (localhost)
- Only accessible from your computer
- Friends can't connect from the internet

**Your router blocks direct access:**
- Your public IP changes
- Router blocks incoming connections (NAT/firewall)
- Port forwarding requires router access

## The Solution: playit.gg Tunnel

playit.gg creates a **tunnel** (bridge) between your local computer and the internet.

### How It Works (Step by Step)

```
Internet Player
      │
      │ Connects to: playit.gg's servers
      │ (e.g., believe-interaction.gl.at.ply.gg:23534)
      │
      ▼
┌─────────────────────────────────────┐
│     playit.gg Cloud Servers         │
│  (Always accessible on internet)    │
│                                     │
│  - Has permanent IP addresses       │
│  - Port forwarding configured       │
│  - Domain names assigned            │
└─────────────────────────────────────┘
      │
      │ Tunnel connection (established by playit.gg app)
      │ Your computer → playit.gg servers
      │
      ▼
┌─────────────────────────────────────┐
│      Your Computer                  │
│                                     │
│  playit.gg App (running)            │
│      │                              │
│      ▼                              │
│  Local Server (127.0.0.1:25565)     │
└─────────────────────────────────────┘
```

### Detailed Explanation

#### 1. **Your Computer Creates Outgoing Connection**

When you run playit.gg:
- playit.gg app opens a connection to playit.gg's servers
- This is an **outgoing** connection (your computer → internet)
- Outgoing connections are **never blocked** by routers/firewalls
- Similar to opening a web browser - it works without port forwarding

#### 2. **playit.gg Creates a Persistent Tunnel**

- playit.gg keeps this connection open (persistent tunnel)
- playit.gg assigns you a public address: `believe-interaction.gl.at.ply.gg:23534`
- This address points to playit.gg's servers, not your computer

#### 3. **When Someone Connects**

When a friend connects to `believe-interaction.gl.at.ply.gg:23534`:

```
Friend's Computer
      │
      │ 1. Connects to playit.gg servers
      │    (playit.gg is always accessible)
      │
      ▼
playit.gg Servers
      │
      │ 2. playit.gg receives the connection
      │    "This is for tunnel XYZ"
      │
      │ 3. playit.gg forwards it through the tunnel
      │    (The persistent connection you created)
      │
      ▼
Your Computer (playit.gg app)
      │
      │ 4. playit.gg app receives the forwarded connection
      │
      │ 5. playit.gg app forwards it to your local server
      │    127.0.0.1:25565
      │
      ▼
Your Minecraft Server
```

## Why This Works Without Port Forwarding

### Traditional Method (Port Forwarding):

```
Internet → Router → Your Computer
         ❌ Router blocks this (incoming connection)
```

**Requires:**
- Router admin access
- Public IP address
- Port forwarding configuration
- Router allows incoming connections

### playit.gg Method (Tunneling):

```
Your Computer → playit.gg Servers (outgoing connection - always works!)
                ↓
Internet → playit.gg Servers → Tunnel → Your Computer
         ✅ Works! (outgoing connection, not incoming)
```

**Why it works:**
- Uses **outgoing connections only** (never blocked)
- playit.gg's servers handle incoming connections
- Your computer just keeps the tunnel open
- No router configuration needed!

## Technical Details

### What playit.gg Does:

1. **Establishes persistent connection:**
   - Your computer connects to playit.gg (outgoing)
   - Connection stays open as long as app is running
   - This is the "tunnel"

2. **Receives connections on your behalf:**
   - playit.gg servers listen on public IP/port
   - When someone connects, playit.gg forwards through tunnel
   - Your computer receives it as if it was direct

3. **Handles protocol translation:**
   - Converts between public internet and local network
   - Handles IP/port mapping
   - Manages connection routing

### Types of Tunnels:

**Java Server (TCP):**
- Uses TCP protocol
- Connection: `localhost:25565` → playit.gg → internet
- Friends connect via playit.gg address

**Bedrock Server (UDP):**
- Uses UDP protocol
- Connection: `localhost:19132` → playit.gg → internet
- Different tunnel (UDP vs TCP)

## Why It's Called "Tunneling"

Think of it like a tunnel:

- **Direct route blocked:** Internet can't reach you directly (router blocks it)
- **Tunnel route works:** Go through playit.gg's servers (outgoing connection)
- **Tunnel bridges the gap:** Makes your local server accessible from internet

## Limitations

### Requires playit.gg App Running:

- **Must keep playit.gg app open** for tunnel to work
- If you close it, tunnel closes, friends can't connect
- Your computer must be online and running

### Latency (Slight Delay):

- Extra hop through playit.gg servers
- Usually adds 20-100ms delay
- Still very playable for most games

### Free Tier Limits:

- May have connection limits
- May have bandwidth limits
- Address may change on restart
- Some regions may be slower

## Comparison

| Method | Port Forwarding | playit.gg Tunnel |
|--------|----------------|------------------|
| **Router access needed?** | ✅ Yes | ❌ No |
| **Always works?** | ✅ Yes (permanent) | ⚠️ Only when app running |
| **Setup difficulty** | Hard | Easy |
| **Latency** | Direct (lowest) | Tunnel (slightly higher) |
| **Public IP needed?** | ✅ Yes | ❌ No |
| **Address changes?** | Only if IP changes | May change on restart |

## Summary

**playit.gg makes your server accessible by:**

1. ✅ Creating an **outgoing connection** (never blocked)
2. ✅ Keeping a **persistent tunnel** open to their servers
3. ✅ Receiving connections on their servers (always accessible)
4. ✅ Forwarding connections through the tunnel to your computer
5. ✅ Your computer receives it locally

**No port forwarding needed** because everything uses outgoing connections!

**Think of it like:** Your computer calls playit.gg and keeps the phone line open. When friends call playit.gg's number, playit.gg forwards the call through your open line to your computer. Your computer never needs to receive direct calls!



