# MineServer GUI

A simple Windows GUI application for managing Minecraft servers.

## Features

- ✅ Start/Stop/Restart server with one click
- ✅ Whitelist management
- ✅ Server settings (max players, difficulty, gamemode, MOTD)
- ✅ Connection info display (Local IP, Public IP)
- ✅ Copy connection URLs to clipboard

## Requirements

- Windows 10/11
- .NET 6.0 Runtime (or build as self-contained)
- Java 21+ installed
- Minecraft server.jar in `../server/` directory

## Building

1. Open `MineServerGUI.sln` in Visual Studio 2022
2. Restore NuGet packages
3. Build solution (F6)

## Running

1. Ensure Minecraft server is set up in `../server/` directory
2. Run `MineServerGUI.exe`
3. Click "Start Server" to begin

## Project Structure

```
MineServerGUI/
├── Forms/
│   └── MainForm.cs          # Main UI window
├── Core/
│   ├── ServerManager.cs     # Server process management
│   └── ConfigManager.cs     # Configuration file management
├── Models/
│   ├── ServerProperties.cs  # Server properties model
│   └── WhitelistEntry.cs   # Whitelist entry model
└── Utilities/
    └── NetworkHelper.cs     # Network utilities
```

## Notes

- Server must be set up first using `scripts/setup/setup-server.ps1`
- Whitelist is enabled by default for security
- Settings are saved automatically when changed

