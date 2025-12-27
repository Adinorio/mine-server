# Instructions for Friend: Fix Firewall on Your Computer

**Send this to your friend who can't connect!**

---

## Quick Test First (2 Minutes)

1. **Temporarily disable Windows Firewall:**
   - Press `Windows Key + I`
   - Go to "Privacy & Security" → "Windows Security"
   - Click "Firewall & network protection"
   - Click on each network (Domain, Private, Public)
   - Turn off "Microsoft Defender Firewall"

2. **Try connecting to the server again**

3. **If it works now:** Follow the steps below to permanently allow it

4. **Re-enable firewall** after testing

---

## Permanent Fix: Allow Minecraft Through Firewall

### Step 1: Open Windows Firewall Settings

1. Press `Windows Key + R`
2. Type: `wf.msc`
3. Press Enter

### Step 2: Create Outbound Rule

1. **Click "Outbound Rules"** (on the left)
2. **Click "New Rule..."** (on the right)

3. **Select "Program"** → Click Next

4. **Select "All programs"** → Click Next
   - (Or browse to your Java/Minecraft installation folder)

5. **Select "Allow the connection"** → Click Next

6. **Check all boxes:** Domain, Private, Public → Click Next

7. **Name it:** "Minecraft - Allow Outbound" → Click Finish

---

## Alternative: Allow Java Specifically

If the above doesn't work, allow Java directly:

1. **Find Java installation:**
   - Usually: `C:\Program Files\Java\jre-XX\bin\java.exe`
   - Or: `C:\Program Files\Java\jdk-XX\bin\java.exe`

2. **Follow steps above, but at Step 4:**
   - Select "This program path:"
   - Browse to `java.exe` file
   - Complete the rest of the steps

---

## Check Antivirus Software

Your antivirus might also be blocking:

### If you have Avast/AVG:
- Open Avast/AVG
- Go to Settings → Exceptions
- Add Minecraft folder or Java folder

### If you have Norton/McAfee/Kaspersky:
- Look for "Firewall" or "Network Protection" settings
- Add Minecraft/Java to allowed programs

### Windows Defender (Built-in):
- Usually covered by firewall fix above
- But check: Windows Security → Virus & threat protection → Manage settings → Exclusions

---

## Still Not Working?

Try these:

1. **Disable VPN** (if you're using one)
2. **Try connecting from a different network** (mobile hotspot)
3. **Check if your router blocks gaming connections**
4. **Contact the server owner** - they might need to check their firewall too

---

## Quick Checklist

- [ ] Disabled firewall temporarily (to test)
- [ ] Created outbound firewall rule for Minecraft
- [ ] Checked antivirus isn't blocking
- [ ] Disabled VPN (if using)
- [ ] Tried different network (if possible)

**If it still doesn't work after all this, the issue might be on the server side or network/router blocking.**

