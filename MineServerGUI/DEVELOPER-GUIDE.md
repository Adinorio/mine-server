# Developer Guide - Accessing Setup Wizard

## How to Access Setup Wizard Again

### Method 1: Delete server.jar (Easiest)
1. Navigate to: `mine-server/server/`
2. Delete or rename `server.jar`
3. Restart the GUI application
4. Setup wizard will appear automatically

### Method 2: Add Developer Menu (Recommended for Testing)

Add a hidden developer menu:

**Steps:**
1. Add a context menu or keyboard shortcut
2. Or add a "Settings" button that includes "Reset Setup"
3. Or press a key combination (like Ctrl+Shift+S)

### Method 3: Programmatically Trigger

In `MainForm.cs`, you can call:
```csharp
CheckServerSetup(); // This will show the wizard if server.jar is missing
```

### Method 4: Add a Menu Item

Add this to MainForm:

```csharp
// In InitializeComponent, add:
var menuStrip = new MenuStrip();
var toolsMenu = new ToolStripMenuItem("Tools");
var resetSetupItem = new ToolStripMenuItem("Reset Setup Wizard");
resetSetupItem.Click += (s, e) => {
    var result = MessageBox.Show(
        "This will delete server.jar and show setup wizard.\nContinue?",
        "Reset Setup",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);
    if (result == DialogResult.Yes)
    {
        var serverJar = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server", "server.jar");
        if (File.Exists(serverJar))
        {
            File.Delete(serverJar);
            CheckServerSetup();
        }
    }
};
toolsMenu.DropDownItems.Add(resetSetupItem);
menuStrip.Items.Add(toolsMenu);
this.MainMenuStrip = menuStrip;
this.Controls.Add(menuStrip);
```

## Quick Test Commands

### PowerShell - Delete server.jar
```powershell
Remove-Item "server\server.jar" -ErrorAction SilentlyContinue
```

### PowerShell - Rename (backup)
```powershell
Rename-Item "server\server.jar" "server\server.jar.backup" -ErrorAction SilentlyContinue
```

### PowerShell - Restore
```powershell
Rename-Item "server\server.jar.backup" "server\server.jar" -ErrorAction SilentlyContinue
```

## Testing Checklist

- [ ] Delete server.jar
- [ ] Launch GUI
- [ ] Setup wizard appears
- [ ] Test "Select File" option
- [ ] Test "Download" option
- [ ] Verify version detection works
- [ ] Test with different server.jar versions

## Debug Tips

1. **Check if wizard appears:**
   - Set breakpoint in `CheckServerSetup()` method
   - Verify `_downloader.ServerJarExists()` returns false

2. **Test version detection:**
   - Use different server.jar files
   - Check console output for version detection

3. **Test download:**
   - Delete server.jar
   - Click download button
   - Monitor progress bar

