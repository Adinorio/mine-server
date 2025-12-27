# Quick ngrok Setup Script
Write-Host "=== ngrok Setup Helper ===" -ForegroundColor Cyan
Write-Host ""

$ngrokPath = ".\ngrok-v3-stable-windows-amd64\ngrok.exe"

if (-not (Test-Path $ngrokPath)) {
    Write-Host "ERROR: ngrok.exe not found!" -ForegroundColor Red
    Write-Host "Expected location: $ngrokPath" -ForegroundColor Yellow
    exit 1
}

Write-Host "ngrok found at: $ngrokPath" -ForegroundColor Green
Write-Host ""

# Check if authtoken is configured (ngrok v3 uses different location)
$ngrokConfig1 = "$env:USERPROFILE\.ngrok2\ngrok.yml"
$ngrokConfig2 = "$env:LOCALAPPDATA\ngrok\ngrok.yml"
$hasConfig = (Test-Path $ngrokConfig1) -or (Test-Path $ngrokConfig2)

if ($hasConfig) {
    Write-Host "ngrok authtoken appears to be configured" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can start the tunnel now!" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Option 1: Use the helper script" -ForegroundColor Yellow
    Write-Host "  .\start-ngrok.ps1" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Option 2: Manual start" -ForegroundColor Yellow
    Write-Host "  cd ngrok-v3-stable-windows-amd64" -ForegroundColor Gray
    Write-Host "  .\ngrok tcp 25565" -ForegroundColor Gray
} else {
    Write-Host "ngrok authtoken not configured yet" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To configure:" -ForegroundColor Cyan
    Write-Host "1. Get your authtoken from:" -ForegroundColor White
    Write-Host "   https://dashboard.ngrok.com/get-started/your-authtoken" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "2. Run this command (replace YOUR_TOKEN):" -ForegroundColor White
    Write-Host "   cd ngrok-v3-stable-windows-amd64" -ForegroundColor Gray
    Write-Host "   .\ngrok config add-authtoken YOUR_TOKEN" -ForegroundColor Gray
    Write-Host ""
    Write-Host "3. Then come back and run this script again" -ForegroundColor White
}

Write-Host ""
Write-Host "=== Quick Start Guide ===" -ForegroundColor Cyan
Write-Host "1. Make sure Minecraft server is running: .\start-server.ps1" -ForegroundColor White
Write-Host "2. Start ngrok tunnel: .\start-ngrok.ps1" -ForegroundColor White
Write-Host "3. Share the address ngrok shows with friends" -ForegroundColor White
