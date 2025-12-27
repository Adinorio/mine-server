# Windows Firewall Rule for Minecraft Server
# Run this script as Administrator

Write-Host "=== Adding Windows Firewall Rule for Minecraft Server ===" -ForegroundColor Cyan
Write-Host ""

# Check if running as administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator!" -ForegroundColor Red
    Write-Host ""
    Write-Host "To fix this:" -ForegroundColor Yellow
    Write-Host "1. Right-click PowerShell" -ForegroundColor White
    Write-Host "2. Select 'Run as Administrator'" -ForegroundColor White
    Write-Host "3. Navigate to this folder" -ForegroundColor White
    Write-Host "4. Run: .\fix-firewall.ps1" -ForegroundColor White
    exit 1
}

# Remove existing rule if it exists
Write-Host "Removing existing Minecraft Server firewall rule (if any)..." -ForegroundColor Yellow
Remove-NetFirewallRule -DisplayName "Minecraft Server - Port 25565" -ErrorAction SilentlyContinue

# Add new firewall rule
Write-Host "Adding firewall rule for port 25565..." -ForegroundColor Yellow
New-NetFirewallRule -DisplayName "Minecraft Server - Port 25565" `
    -Direction Inbound `
    -LocalPort 25565 `
    -Protocol TCP `
    -Action Allow `
    -Description "Allows Minecraft Server connections on port 25565" | Out-Null

Write-Host ""
Write-Host "Firewall rule added successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Your server should now be accessible from other devices on your network." -ForegroundColor Cyan
Write-Host ""
Write-Host "To connect from another device:" -ForegroundColor Yellow
Write-Host "1. Make sure the server is running (.\start-server.ps1)" -ForegroundColor White
Write-Host "2. Use your local IP address (shown when server starts)" -ForegroundColor White
Write-Host "3. Or use: 192.168.1.8:25565 (your WiFi IP)" -ForegroundColor White











