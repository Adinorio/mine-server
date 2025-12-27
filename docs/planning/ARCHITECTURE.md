# Clean Architecture Structure

```
mine-server/
├── MineServerGUI/              # GUI Application (C# WinForms)
│   ├── MineServerGUI.sln
│   ├── MineServerGUI/
│   │   ├── Forms/
│   │   ├── Core/
│   │   ├── Models/
│   │   └── Utilities/
│   └── README.md
│
├── scripts/                    # PowerShell Scripts
│   ├── setup/
│   ├── server/
│   ├── geyser/
│   └── utilities/
│
├── docs/                      # Documentation
│   ├── guides/
│   ├── troubleshooting/
│   └── planning/
│
├── server/                    # Minecraft Server Files
│   ├── server.jar
│   ├── server.properties
│   ├── world/
│   └── logs/
│
├── geyser/                    # GeyserMC Files
│   ├── geyser.jar
│   └── config.yml
│
├── backups/                   # World Backups
│
├── README.md                  # Main README
└── .gitignore
```

