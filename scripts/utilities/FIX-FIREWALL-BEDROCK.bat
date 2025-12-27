@echo off
echo ========================================
echo Minecraft Bedrock Firewall Fix
echo ========================================
echo.
echo This will add a firewall rule for Bedrock port 19132 (UDP)
echo.
echo This is needed for GeyserMC to allow Bedrock players to join
echo.
pause

powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "& {Remove-NetFirewallRule -DisplayName 'Minecraft Bedrock - Port 19132' -ErrorAction SilentlyContinue; New-NetFirewallRule -DisplayName 'Minecraft Bedrock - Port 19132' -Direction Inbound -LocalPort 19132 -Protocol UDP -Action Allow -Description 'Allows Minecraft Bedrock players to connect via GeyserMC on port 19132'; Write-Host ''; Write-Host 'Firewall rule added successfully!' -ForegroundColor Green; Write-Host 'Bedrock players should now be able to connect!' -ForegroundColor Green; Write-Host ''; Write-Host 'Press any key to exit...'; $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')}"



