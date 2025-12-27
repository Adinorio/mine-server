# System Architecture

## Overview

MineServer is a Windows-based Minecraft server management system with a GUI application and automated scripts for easy server hosting.

## Directory Structure

```
mine-server/
├── MineServerGUI/              # C# WinForms GUI Application
│   ├── MineServerGUI.sln
│   └── MineServerGUI/
│       ├── Forms/              # UI Forms (MainForm, SetupWizard, etc.)
│       ├── Core/               # Core logic (ServerManager, ConfigManager, etc.)
│       ├── Models/             # Data models
│       ├── Utilities/          # Helper classes
│       └── server-profiles/    # Server profile configurations
│
├── scripts/                    # PowerShell Scripts
│   ├── setup/                  # Initial setup scripts
│   ├── server/                 # Server management (start/stop)
│   ├── geyser/                 # GeyserMC integration
│   └── utilities/              # Utility scripts
│
├── docs/                       # Documentation
│   ├── guides/                 # User guides
│   ├── troubleshooting/        # Troubleshooting guides
│   └── planning/               # Project planning docs
│
├── server/                     # Minecraft Server (IGNORED)
│   ├── server.jar
│   ├── server.properties
│   ├── world/
│   └── logs/
│
├── geyser/                     # GeyserMC Files (IGNORED)
│   ├── geyser.jar
│   └── config.yml
│
├── backups/                    # World Backups (IGNORED)
│
├── libraries/                  # Shared libraries (IGNORED)
├── versions/                   # Server version cache (IGNORED)
│
├── README.md                   # Main documentation
├── ARCHITECTURE.md             # This file
└── .gitignore                  # Git ignore rules
```

## Core Components

### 1. GUI Application (MineServerGUI)

**Technology:** C# WinForms (.NET 8.0)

**Main Components:**
- **Forms/**: User interface forms
  - `MainForm.cs` - Main control panel
  - `SetupWizardForm.cs` - Initial setup wizard
  - `ServerProfileForm.cs` - Server profile management
  - `ChangeVersionForm.cs` - Version management

- **Core/**: Business logic
  - `ServerManager.cs` - Server process management (start/stop/restart)
  - `ServerProfileManager.cs` - Profile configuration management
  - `ConfigManager.cs` - server.properties file management
  - `ServerDownloader.cs` - Server JAR downloader with caching
  - `ServerVersionDetector.cs` - Version detection

- **Models/**: Data structures
  - `ServerProfile.cs` - Server profile model
  - `ServerProperties.cs` - Server properties model
  - `WhitelistEntry.cs` - Whitelist entry model

- **Utilities/**: Helper classes
  - `JavaVersionChecker.cs` - Java version detection
  - `NetworkHelper.cs` - Network utilities
  - `ServerBackupManager.cs` - Backup management
  - `VersionHelper.cs` - Version utilities

### 2. PowerShell Scripts

**Location:** `scripts/`

- **setup/**: Initial server setup and configuration
- **server/**: Server lifecycle management (start, stop, restart)
- **geyser/**: GeyserMC proxy management
- **utilities/**: Helper scripts and tools

### 3. Documentation

**Location:** `docs/`

- **guides/**: Step-by-step user guides
- **troubleshooting/**: Problem-solving guides
- **planning/**: Project planning and technical specs

## Data Flow

```
User → GUI Application → ServerManager → Java Process (server.jar)
                    ↓
              ConfigManager → server.properties
                    ↓
              ProfileManager → profiles.json
```

## Ignored Files

The following directories/files are excluded from version control (see `.gitignore`):

- `/server/` - Entire server directory (world, server.jar, logs, libraries)
- `/backups/` - World backups
- `/geyser/geyser.jar` - GeyserMC JAR file
- `/libraries/` - Downloaded libraries
- `/versions/` - Server version cache
- Log files, cache files, and other runtime artifacts

**Exception:** `MineServerGUI/MineServerGUI/server/` is NOT ignored (needed for the application).

## Key Features

- **Server Management**: Start, stop, restart server via GUI
- **Profile System**: Multiple server profiles with different versions
- **Version Management**: Download and switch between Minecraft versions
- **Configuration**: GUI-based server.properties editing
- **Whitelist Management**: Add/remove players from whitelist
- **Network Integration**: Connection info display and URL copying
- **Backup System**: Automatic world backups
- **GeyserMC Support**: Bedrock crossplay integration

## Technology Stack

- **GUI**: C# WinForms (.NET 8.0)
- **Server**: Minecraft Vanilla Server (Java)
- **Proxy**: GeyserMC (Bedrock support)
- **Scripts**: PowerShell 5.1+
- **Java**: JDK 21+ required

## Build Requirements

- Visual Studio 2022 (for GUI development)
- .NET 8.0 SDK
- Java 21+ (for running server)
- Windows 10/11
