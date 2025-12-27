# MineServer - Minecraft Server Management

A complete solution for hosting Minecraft servers on Windows, featuring a GUI application and automated setup scripts.

## 🚀 Quick Start

### Option 1: GUI Application (Recommended)

1. **Build the GUI:**
   - Open `MineServerGUI/MineServerGUI.sln` in Visual Studio 2022
   - Build the solution (F6)
   - Run `MineServerGUI.exe`

2. **First-Time Setup:**
   - Run `scripts/setup/setup-server.ps1` to set up the server
   - The GUI will handle the rest!

### Option 2: PowerShell Scripts

1. **Setup Server:**
   ```powershell
   .\scripts\setup\setup-server.ps1
   ```

2. **Start Server:**
   ```powershell
   .\scripts\server\start-server.ps1
   ```

## 📁 Project Structure

```
mine-server/
├── MineServerGUI/          # GUI Application (C# WinForms)
├── scripts/               # PowerShell Scripts
│   ├── setup/             # Setup scripts
│   ├── server/           # Server management
│   ├── geyser/           # GeyserMC scripts
│   └── utilities/        # Utility scripts
├── docs/                  # Documentation
│   ├── guides/           # Setup guides
│   ├── troubleshooting/  # Troubleshooting guides
│   └── planning/        # Project planning docs
├── server/                # Minecraft Server Files
├── geyser/                # GeyserMC Files
└── backups/              # World Backups
```

## ✨ Features

### GUI Application
- ✅ One-click server start/stop/restart
- ✅ Whitelist management
- ✅ Server settings (max players, difficulty, gamemode, MOTD)
- ✅ Connection info display
- ✅ Copy connection URLs to clipboard

### Server Features
- ✅ Vanilla Minecraft server
- ✅ TLauncher compatible
- ✅ GeyserMC for Bedrock crossplay
- ✅ playit.gg tunneling (no port forwarding needed)
- ✅ Automatic backups

## 📋 Requirements

- Windows 10/11
- Java 21+ ([Download](https://adoptium.net/))
- .NET 6.0 Runtime (for GUI, or build as self-contained)
- Visual Studio 2022 (for building GUI)

## 🔐 Security

- Whitelist enabled by default
- Automatic backups
- No IP exposure (playit.gg tunneling)
- Rate limiting configured

## 📚 Documentation

- **Setup Guides:** `docs/guides/`
- **Troubleshooting:** `docs/troubleshooting/`
- **Project Planning:** `docs/planning/`

## 🎯 Target Use Case

Designed for **close friends** with:
- Windows PC/laptop
- "Okay" WiFi and hardware
- Non-technical users
- Want simple server hosting

## 📝 License

This project is for personal use. Minecraft is a trademark of Mojang Studios.
