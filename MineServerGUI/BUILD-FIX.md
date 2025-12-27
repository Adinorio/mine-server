# Fix Build Error: File Locked by Process

## Problem
Build fails because `MineServerGUI.exe` is locked by process "vgc (11932)".

## Quick Fix

### Option 1: Close the Running Application (Easiest)
1. **Close MineServerGUI.exe** if it's running
2. **Close Visual Studio Code** if it has the file open
3. **Rebuild** (F6)

### Option 2: Kill the Process
1. Open **Task Manager** (Ctrl+Shift+Esc)
2. Find process "vgc" or PID 11932
3. **End Task**
4. **Rebuild**

### Option 3: Stop Debugging
1. In Visual Studio, click **Stop Debugging** (Shift+F5)
2. Wait a few seconds
3. **Rebuild** (F6)

## Prevention

**Before building:**
- Always stop the application first
- Close any file explorers showing the exe
- Close Visual Studio Code if it's open

## Alternative: Build Configuration

You can also change build settings to avoid this:
- Build → Configuration Manager
- Uncheck "Deploy" if enabled
- Or change output path to a different location

