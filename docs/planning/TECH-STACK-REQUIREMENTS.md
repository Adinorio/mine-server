# Technology Stack & Requirements

## 🎯 Recommended Stack: **C# WinForms** ⭐ BEST CHOICE

### **Why C# WinForms?**

**Pros:**
- ✅ Native Windows performance (fast, lightweight)
- ✅ Small executable size (~5-10MB without Java bundling)
- ✅ Easy PowerShell integration (native .NET)
- ✅ Built-in file system operations
- ✅ Process management (start/stop server)
- ✅ Windows Forms = simple, familiar UI
- ✅ No web browser overhead (unlike Electron)
- ✅ Fast startup time (< 1 second)
- ✅ Good Windows integration (firewall, services)

**Cons:**
- ❌ Windows-only (but that's fine for your use case)
- ❌ UI looks "Windows classic" (but functional)

**Alternative Considered:**
- **WPF** - More modern UI, but more complex
- **Electron** - Too heavy (~100MB), slower startup
- **Python + Tkinter** - Slower, larger bundle

---

## 📦 Development Requirements

### **For Development:**

1. **Visual Studio 2022** (Community Edition - FREE)
   - Download: https://visualstudio.microsoft.com/downloads/
   - Select: ".NET desktop development" workload
   - Includes: C# compiler, WinForms designer, debugger

2. **.NET 6.0 SDK or later** (included with Visual Studio)
   - Or download separately: https://dotnet.microsoft.com/download

3. **Windows 10/11** (development machine)

**That's it!** No other dependencies needed for development.

---

## 🎨 UI Framework: **Windows Forms (WinForms)**

### **Why WinForms?**
- ✅ Simple drag-and-drop designer
- ✅ Fast development
- ✅ Native Windows look
- ✅ Good enough for this use case
- ✅ No learning curve if you know basic C#

### **Alternative: WPF** (if you want modern UI)
- More modern appearance
- Better animations
- More complex to learn
- Overkill for this project

**Recommendation:** Start with WinForms, upgrade to WPF later if needed.

---

## 🔧 Core Libraries & Dependencies

### **Built-in (.NET Framework):**
- ✅ `System.Diagnostics.Process` - Start/stop server
- ✅ `System.IO` - File operations (read/write configs)
- ✅ `System.Net.NetworkInformation` - Get local IP
- ✅ `System.Windows.Forms` - GUI components
- ✅ `System.Text.RegularExpressions` - Parse logs
- ✅ `System.Management.Automation` - PowerShell integration

### **NuGet Packages (Optional):**

1. **Newtonsoft.Json** (for JSON parsing)
   ```bash
   Install-Package Newtonsoft.Json
   ```
   - Parse `whitelist.json`, `ops.json`
   - Read/write config files

2. **System.Management.Automation** (PowerShell)
   ```bash
   Install-Package System.Management.Automation
   ```
   - Execute PowerShell scripts
   - Run backup scripts

**That's it!** Minimal dependencies = smaller executable.

---

## 📁 Project Structure

```
MineServerGUI/
├── MineServerGUI.sln                    # Solution file
├── MineServerGUI/
│   ├── MineServerGUI.csproj              # Project file
│   ├── Program.cs                        # Entry point
│   │
│   ├── Forms/                            # UI Forms
│   │   ├── MainForm.cs                   # Main window
│   │   ├── MainForm.Designer.cs          # UI designer code
│   │   ├── SettingsForm.cs               # Settings window
│   │   └── PlayerManagementForm.cs       # Player management
│   │
│   ├── Core/                             # Business logic
│   │   ├── ServerManager.cs              # Start/stop server
│   │   ├── ConfigManager.cs              # Read/write configs
│   │   ├── LogParser.cs                  # Parse server logs
│   │   └── ProcessMonitor.cs             # Monitor server process
│   │
│   ├── Models/                           # Data models
│   │   ├── ServerProperties.cs           # server.properties model
│   │   ├── WhitelistEntry.cs             # Whitelist model
│   │   └── Player.cs                     # Player model
│   │
│   ├── Utilities/                        # Helper classes
│   │   ├── PowerShellRunner.cs          # Execute PowerShell
│   │   ├── BackupManager.cs              # Backup operations
│   │   ├── NetworkHelper.cs              # Get IP addresses
│   │   └── FileHelper.cs                # File operations
│   │
│   └── Resources/                        # Resources
│       ├── Icons/                        # App icons
│       └── Configs/                     # Default configs
│
└── Tests/                                # Unit tests (optional)
    └── MineServerGUI.Tests.csproj
```

---

## 🚀 Getting Started Steps

### **Step 1: Install Visual Studio**

1. Download Visual Studio 2022 Community (free)
2. Run installer
3. Select workload: **".NET desktop development"**
4. Install

### **Step 2: Create Project**

1. Open Visual Studio
2. File → New → Project
3. Template: **"Windows Forms App (.NET)"**
4. Name: `MineServerGUI`
5. Framework: **.NET 6.0** or **.NET 8.0**
6. Create

### **Step 3: Add NuGet Packages**

```powershell
# In Package Manager Console (Tools → NuGet Package Manager → Package Manager Console)

Install-Package Newtonsoft.Json
Install-Package System.Management.Automation
```

### **Step 4: Set Up Project Structure**

Create folders:
- `Forms/`
- `Core/`
- `Models/`
- `Utilities/`
- `Resources/`

### **Step 5: Start Building!**

Begin with `MainForm.cs` - the main window.

---

## 💻 Code Example: Basic Server Manager

### **ServerManager.cs** (Core functionality):

```csharp
using System;
using System.Diagnostics;
using System.IO;

namespace MineServerGUI.Core
{
    public class ServerManager
    {
        private Process serverProcess;
        private string serverPath = "server";
        private string serverJar = "server/server.jar";
        
        public bool IsRunning => serverProcess != null && !serverProcess.HasExited;
        
        public void StartServer(string javaPath, string minMemory, string maxMemory)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Server is already running!");
            }
            
            if (!File.Exists(serverJar))
            {
                throw new FileNotFoundException($"Server JAR not found: {serverJar}");
            }
            
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = javaPath,
                Arguments = $"-Xms{minMemory} -Xmx{maxMemory} -jar server.jar nogui",
                WorkingDirectory = Path.GetFullPath(serverPath),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false
            };
            
            serverProcess = Process.Start(startInfo);
        }
        
        public void StopServer()
        {
            if (!IsRunning)
            {
                return;
            }
            
            // Send "stop" command to server
            serverProcess.StandardInput.WriteLine("stop");
            serverProcess.WaitForExit(5000);
            
            // Force kill if still running
            if (!serverProcess.HasExited)
            {
                serverProcess.Kill();
            }
            
            serverProcess.Dispose();
            serverProcess = null;
        }
    }
}
```

### **MainForm.cs** (UI):

```csharp
using System;
using System.Windows.Forms;
using MineServerGUI.Core;

namespace MineServerGUI.Forms
{
    public partial class MainForm : Form
    {
        private ServerManager serverManager;
        
        public MainForm()
        {
            InitializeComponent();
            serverManager = new ServerManager();
            UpdateUI();
        }
        
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                serverManager.StartServer("java", "2G", "4G");
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting server: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                serverManager.StopServer();
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping server: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void UpdateUI()
        {
            bool isRunning = serverManager.IsRunning;
            btnStart.Enabled = !isRunning;
            btnStop.Enabled = isRunning;
            lblStatus.Text = isRunning ? "● Running" : "● Stopped";
            lblStatus.ForeColor = isRunning ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }
    }
}
```

---

## 📦 Distribution Requirements

### **For End Users:**

**Option 1: Self-Contained Deployment** (Recommended)
- ✅ Includes .NET runtime
- ✅ No installation needed
- ✅ Larger file (~50-70MB)
- ✅ Works on any Windows 10/11

**Option 2: Framework-Dependent**
- ✅ Smaller file (~5-10MB)
- ❌ Requires .NET runtime installed
- ❌ Users need to install .NET first

**Recommendation:** Self-contained for ease of use.

---

## 🔐 Code Signing (Optional but Recommended)

### **Why Code Sign?**
- Prevents Windows Defender warnings
- Builds user trust
- Professional appearance

### **How:**
1. Buy code signing certificate (~$200-400/year)
2. Or use self-signed (free, but shows warning)

**For MVP:** Can skip, add later.

---

## 📊 Build Output

### **Release Build:**
- Single `.exe` file (self-contained)
- Size: ~50-70MB (includes .NET runtime)
- Or: ~5-10MB (framework-dependent)

### **Distribution:**
- Zip file with `.exe`
- Include README.txt with instructions
- Optional: Installer (NSIS/InnoSetup)

---

## 🛠️ Development Tools

### **Essential:**
- ✅ Visual Studio 2022 (IDE)
- ✅ Git (version control)

### **Optional:**
- ⏳ JetBrains Rider (alternative IDE)
- ⏳ WinMerge (file comparison)
- ⏳ ILSpy (decompile .NET assemblies)

---

## 📋 Minimum System Requirements

### **Development Machine:**
- Windows 10/11
- 4GB RAM (8GB recommended)
- 2GB free disk space
- Visual Studio 2022

### **End User Machine:**
- Windows 10/11
- 2GB RAM
- 100MB free disk space (for GUI)
- Java 21+ (for server)

---

## ✅ Quick Start Checklist

- [ ] Install Visual Studio 2022 Community
- [ ] Install .NET 6.0+ SDK
- [ ] Create new WinForms project
- [ ] Add NuGet packages (Newtonsoft.Json, PowerShell)
- [ ] Set up project structure
- [ ] Create ServerManager class
- [ ] Create MainForm UI
- [ ] Test start/stop functionality

---

## 🎯 Technology Decision Summary

| Aspect | Choice | Why |
|--------|--------|-----|
| **Language** | C# | Native Windows, easy PowerShell integration |
| **UI Framework** | WinForms | Simple, fast development |
| **Runtime** | .NET 6.0+ | Modern, cross-platform potential (future) |
| **JSON Library** | Newtonsoft.Json | Industry standard, easy to use |
| **PowerShell** | System.Management.Automation | Native .NET integration |
| **Distribution** | Self-contained .exe | No installation needed |

---

## 🚀 Next Steps

1. **Install Visual Studio 2022**
2. **Create project** (WinForms, .NET 6.0)
3. **Set up structure** (folders, classes)
4. **Build MVP** (start with ServerManager)

**Ready to start coding!** 🎉

