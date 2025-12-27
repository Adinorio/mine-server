@echo off
echo ========================================
echo Minecraft Server Firewall Fix
echo ========================================
echo.
echo This will add a firewall rule for port 25565
echo.
pause

powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "& {Remove-NetFirewallRule -DisplayName 'Minecraft Server - Port 25565' -ErrorAction SilentlyContinue; New-NetFirewallRule -DisplayName 'Minecraft Server - Port 25565' -Direction Inbound -LocalPort 25565 -Protocol TCP -Action Allow -Description 'Allows Minecraft Server connections on port 25565'; Write-Host ''; Write-Host 'Firewall rule added successfully!' -ForegroundColor Green; Write-Host 'Server should now be accessible!' -ForegroundColor Green; Write-Host ''; Write-Host 'Press any key to exit...'; $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')}"








