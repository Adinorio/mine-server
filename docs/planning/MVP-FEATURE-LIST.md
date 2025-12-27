# MVP Feature List - Focused on Friend Groups

## 🎯 Core Philosophy

**"Make it so simple that your least technical friend can set up a server in 5 minutes."**

---

## ✅ MVP Features (Phase 1)

### **1. Server Control** ⭐ CRITICAL

**What:**
- Start Server button
- Stop Server button
- Restart Server button
- Server status indicator (● Green = Running, ● Red = Stopped)

**Why:**
- Most basic need
- Visual feedback is essential
- One-click operation

**Implementation:**
- Monitor Java process
- Show status in real-time
- Disable buttons when appropriate

---

### **2. Whitelist Management** ⭐ CRITICAL

**What:**
- Whitelist toggle (ON/OFF switch)
- Add player button (text input)
- Remove player button (select from list)
- Whitelist player list display

**Why:**
- Security is essential
- Friends need to be added easily
- Visual list prevents confusion

**Implementation:**
- Read/write `whitelist.json`
- Send `/whitelist add/remove` commands
- Default: ON for new servers
- Warning if whitelist is empty

---

### **3. Connection Info Display** ⭐ CRITICAL

**What:**
- Local IP display (e.g., `192.168.1.8:25565`)
- playit.gg URL display (e.g., `abc123.playit.gg:25565`)
- Copy URL button (one-click copy to clipboard)
- Connection status (Connected/Disconnected)

**Why:**
- Friends need the URL to connect
- Copy button prevents typos
- Shows both options (local + online)

**Implementation:**
- Detect local IP automatically
- Read playit.gg URL from config or API
- Windows clipboard integration

---

### **4. Player Management** ⭐ IMPORTANT

**What:**
- Current player list (who's online)
- Player count (e.g., "3/8 players")
- Add OP button (make player admin)
- Remove OP button

**Why:**
- See who's playing
- Friends might need OP for commands
- Simple admin management

**Implementation:**
- Parse server logs for player events
- Read/write `ops.json`
- Real-time updates

---

### **5. Automatic Backups** ⭐ IMPORTANT

**What:**
- Backup status (last backup time)
- Manual backup button
- Backup list (with dates)
- Restore button (select backup to restore)

**Why:**
- Prevents world loss
- Easy recovery from griefing
- Peace of mind

**Implementation:**
- Integrate with `backup-world.ps1`
- Schedule automatic backups (every 30 min)
- Display backup list with timestamps

---

### **6. Basic Settings** ⭐ NICE TO HAVE

**What:**
- Max players slider (1-20)
- Difficulty selector (Easy/Normal/Hard)
- Gamemode selector (Survival/Creative)
- MOTD text input (server description)

**Why:**
- Common customizations
- Visual controls are easier than editing files
- Covers most use cases

**Implementation:**
- Read/write `server.properties`
- Validate inputs
- Apply on server restart

---

## 🚫 NOT in MVP (Add Later)

### **Phase 2 Features:**
- ❌ Geyser GUI integration (works automatically, no GUI needed yet)
- ❌ Advanced server.properties editor
- ❌ Performance tuning
- ❌ Mod/plugin manager
- ❌ World management
- ❌ Log viewer (console output is enough for MVP)

### **Why Not:**
- Keep MVP focused
- Get core working perfectly first
- Add complexity later

---

## 🎨 UI Layout (MVP)

### **Main Window:**

```
┌─────────────────────────────────────────────┐
│  MineServer GUI                    [X]     │
├─────────────────────────────────────────────┤
│                                             │
│  Server Status:  ● Running                 │
│                                             │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐ │
│  │  START   │  │   STOP   │  │ RESTART  │ │
│  └──────────┘  └──────────┘  └──────────┘ │
│                                             │
│  ┌───────────────────────────────────────┐ │
│  │  Players: 3/8                         │ │
│  │  • Friend1                            │ │
│  │  • Friend2                            │ │
│  │  • Friend3                            │ │
│  └───────────────────────────────────────┘ │
│                                             │
│  ┌───────────────────────────────────────┐ │
│  │  Connection Info                       │ │
│  │  Local: 192.168.1.8:25565            │ │
│  │  Online: abc123.playit.gg:25565      │ │
│  │  [Copy URL]                           │ │
│  └───────────────────────────────────────┘ │
│                                             │
│  ┌───────────────────────────────────────┐ │
│  │  Whitelist: ● ON                      │ │
│  │  [Add Player] [Remove Player]         │ │
│  │  • Friend1                            │ │
│  │  • Friend2                            │ │
│  │  • Friend3                            │ │
│  └───────────────────────────────────────┘ │
│                                             │
│  ┌───────────────────────────────────────┐ │
│  │  Last Backup: 2 minutes ago           │ │
│  │  [Backup Now] [Restore...]            │ │
│  └───────────────────────────────────────┘ │
│                                             │
│  [Settings]                                 │
└─────────────────────────────────────────────┘
```

### **Settings Window (Simple):**

```
┌─────────────────────────────────────────────┐
│  Server Settings                    [X]     │
├─────────────────────────────────────────────┤
│                                             │
│  Max Players:  [8] ──────●─────── [20]    │
│                                             │
│  Difficulty:   ○ Easy  ● Normal  ○ Hard    │
│                                             │
│  Gamemode:     ● Survival  ○ Creative      │
│                                             │
│  Server Message (MOTD):                    │
│  ┌─────────────────────────────────────┐   │
│  │ A Minecraft Server                  │   │
│  └─────────────────────────────────────┘   │
│                                             │
│  [Save] [Cancel]                            │
└─────────────────────────────────────────────┘
```

---

## 🔧 Technical Requirements (MVP)

### **Must Work:**
1. ✅ Start/stop Minecraft server process
2. ✅ Read/write `server.properties`
3. ✅ Read/write `whitelist.json`
4. ✅ Read/write `ops.json`
5. ✅ Parse server logs (latest.log)
6. ✅ Execute backup script
7. ✅ Detect local IP address
8. ✅ Copy to clipboard

### **Nice to Have:**
- ⏳ playit.gg API integration (if available)
- ⏳ Auto-detect playit.gg URL
- ⏳ Server console input (for advanced users)

---

## 📋 MVP Checklist

### **Setup:**
- [ ] First-run wizard (check Java, download server.jar)
- [ ] Create server directory structure
- [ ] Generate world (first start)
- [ ] Configure playit.gg (instructions or auto)

### **Core Functionality:**
- [ ] Start server
- [ ] Stop server
- [ ] Restart server
- [ ] Monitor server status
- [ ] Display connection info

### **Player Management:**
- [ ] Whitelist toggle
- [ ] Add player to whitelist
- [ ] Remove player from whitelist
- [ ] Display whitelist
- [ ] Show online players
- [ ] Add/remove OP

### **Backups:**
- [ ] Manual backup
- [ ] Automatic backups (every 30 min)
- [ ] Display backup list
- [ ] Restore from backup

### **Settings:**
- [ ] Max players
- [ ] Difficulty
- [ ] Gamemode
- [ ] MOTD

### **Polish:**
- [ ] Error messages (clear, helpful)
- [ ] Tooltips (explain features)
- [ ] Status indicators (visual feedback)
- [ ] Confirmation dialogs (destructive actions)

---

## 🎯 Success Metrics

### **User Experience:**
- ✅ Friend sets up server in < 5 minutes
- ✅ Zero command-line usage
- ✅ One-click to start
- ✅ Friends connect successfully

### **Technical:**
- ✅ Server starts reliably
- ✅ No crashes during normal use
- ✅ Backups work correctly
- ✅ Whitelist prevents unauthorized access

### **Support:**
- ✅ Zero "how do I run PowerShell?" questions
- ✅ Zero "where do I find the IP?" questions
- ✅ Clear error messages with solutions

---

## 🚀 Development Priority

### **Week 1:**
1. Basic UI layout
2. Server start/stop functionality
3. Status monitoring

### **Week 2:**
4. Whitelist management
5. Player list display
6. Connection info display

### **Week 3:**
7. Backup system
8. Basic settings
9. Error handling

### **Week 4:**
10. Polish and testing
11. Setup wizard
12. Documentation

---

## ✅ MVP Definition

**MVP = Minimum features needed for a friend to:**
1. ✅ Set up server (with wizard)
2. ✅ Start server (one click)
3. ✅ Add friends to whitelist
4. ✅ Share connection URL
5. ✅ See who's online
6. ✅ Backup world automatically
7. ✅ Change basic settings

**Everything else can wait!**

