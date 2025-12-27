@echo off
echo ========================================
echo Minecraft Server Firewall Fix (For Friends)
echo ========================================
echo.
echo This will allow Minecraft/Java to connect through your firewall.
echo.
echo Press any key to continue (or Ctrl+C to cancel)...
pause >nul

echo.
echo Checking administrator privileges...
net session >nul 2>&1
if %errorLevel% == 0 (
    echo Administrator privileges confirmed!
    goto :runFix
) else (
    echo.
    echo ERROR: This script needs Administrator privileges!
    echo.
    echo To fix this:
    echo 1. Right-click this file (FIX-FRIEND-FIREWALL.bat)
    echo 2. Select "Run as administrator"
    echo 3. Click "Yes" when asked for permission
    echo.
    pause
    exit /b 1
)

:runFix
echo.
echo Adding firewall rule to allow Minecraft/Java connections...
echo.

powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "& {Remove-NetFirewallRule -DisplayName 'Minecraft - Allow Outbound' -ErrorAction SilentlyContinue; Remove-NetFirewallRule -DisplayName 'Minecraft - Allow Outbound Connection' -ErrorAction SilentlyContinue; New-NetFirewallRule -DisplayName 'Minecraft - Allow Outbound Connection' -Direction Outbound -Action Allow -Description 'Allows outbound connections for Minecraft servers' | Out-Null; $javaPaths = @('C:\Program Files\Java', 'C:\Program Files (x86)\Java'); foreach ($javaPath in $javaPaths) { if (Test-Path $javaPath) { $javaFiles = Get-ChildItem -Path \"$javaPath\*\bin\java.exe\" -ErrorAction SilentlyContinue; foreach ($java in $javaFiles) { New-NetFirewallRule -DisplayName \"Minecraft - Allow Java Outbound\" -Direction Outbound -Program $java.FullName -Action Allow -Description 'Allows Java to connect outbound for Minecraft' -ErrorAction SilentlyContinue | Out-Null; } } }; Write-Host ''; Write-Host 'Firewall rules added successfully!' -ForegroundColor Green; Write-Host ''; Write-Host 'Minecraft should now be able to connect to servers!' -ForegroundColor Green; Write-Host ''}"

if %errorLevel% == 0 (
    echo.
    echo ========================================
    echo SUCCESS! Firewall is now configured!
    echo ========================================
    echo.
    echo You can now try connecting to the server again.
    echo.
) else (
    echo.
    echo ========================================
    echo Attempting alternative method...
    echo ========================================
    echo.
    powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "& {New-NetFirewallRule -DisplayName 'Minecraft - Allow Outbound Connection' -Direction Outbound -Action Allow -Description 'Allows outbound connections for Minecraft' | Out-Null; Write-Host 'Alternative firewall rule added!' -ForegroundColor Green}"
)

echo.
echo ========================================
echo IMPORTANT: Check Antivirus Too!
echo ========================================
echo.
echo If you still can't connect, check your antivirus software:
echo - Windows Defender: Usually OK after firewall fix
echo - Avast/AVG/Norton/McAfee: May need to add Minecraft to exceptions
echo.
echo Also try:
echo - Disable VPN if you're using one
echo - Try connecting from a different network (mobile hotspot)
echo.
echo Press any key to exit...
pause >nul

