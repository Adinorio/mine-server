# Check Player Ping on Minecraft Server
# This script shows how to check player ping using server commands

Write-Host "=== Player Ping Checker ===" -ForegroundColor Cyan
Write-Host ""

# Check if server is running
Write-Host "Checking if server is running..." -ForegroundColor Yellow
$serverRunning = Test-NetConnection -ComputerName 127.0.0.1 -Port 25565 -InformationLevel Quiet -WarningAction SilentlyContinue

if (-not $serverRunning) {
    Write-Host "❌ Server is NOT running!" -ForegroundColor Red
    Write-Host "   Start server with: .\start-server.ps1" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Server is running" -ForegroundColor Green
Write-Host ""

Write-Host "=== Methods to Check Player Ping ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "Method 1: In-Game Commands (Best Option)" -ForegroundColor Yellow
Write-Host "  While in the server, type these commands in chat:" -ForegroundColor White
Write-Host ""
Write-Host "  /list" -ForegroundColor Green
Write-Host "    → Shows all online players" -ForegroundColor Gray
Write-Host "    → Some servers show ping next to player names" -ForegroundColor Gray
Write-Host ""
Write-Host "  /ping [player]" -ForegroundColor Green
Write-Host "    → Check your own ping: /ping" -ForegroundColor Gray
Write-Host "    → Check another player: /ping PlayerName" -ForegroundColor Gray
Write-Host "    → Shows latency in milliseconds (ms)" -ForegroundColor Gray
Write-Host ""

Write-Host "Method 2: Server Console (Admin/OPS Only)" -ForegroundColor Yellow
Write-Host "  In the server console window, type:" -ForegroundColor White
Write-Host ""
Write-Host "  list" -ForegroundColor Green
Write-Host "    → Shows all online players" -ForegroundColor Gray
Write-Host ""
Write-Host "  ping [player]" -ForegroundColor Green
Write-Host "    → Check player ping from console" -ForegroundColor Gray
Write-Host ""

Write-Host "Method 3: Multiplayer Menu (For Players)" -ForegroundColor Yellow
Write-Host "  In Minecraft:" -ForegroundColor White
Write-Host "  1. Open Multiplayer menu" -ForegroundColor Gray
Write-Host "  2. Look at server list" -ForegroundColor Gray
Write-Host "  3. Ping shows as bars: 1-5 bars (more = better)" -ForegroundColor Gray
Write-Host "  4. Hover over server to see exact ping (ms)" -ForegroundColor Gray
Write-Host ""

Write-Host "Method 4: Tab List (In-Game)" -ForegroundColor Yellow
Write-Host "  While playing:" -ForegroundColor White
Write-Host "  1. Press TAB key" -ForegroundColor Gray
Write-Host "  2. See player list with ping bars" -ForegroundColor Gray
Write-Host "  3. Ping bars show connection quality" -ForegroundColor Gray
Write-Host ""

Write-Host "=== Understanding Ping Values ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "  < 50ms  = Excellent (green bars)" -ForegroundColor Green
Write-Host "  50-100ms = Good (yellow/green bars)" -ForegroundColor Yellow
Write-Host "  100-200ms = Fair (yellow bars)" -ForegroundColor Yellow
Write-Host "  200-300ms = Poor (orange bars)" -ForegroundColor Red
Write-Host "  > 300ms  = Very Poor (red bars)" -ForegroundColor Red
Write-Host ""

Write-Host "=== Note ===" -ForegroundColor Cyan
Write-Host "If /ping command doesn't work, your server might need:" -ForegroundColor Yellow
Write-Host "  - Minecraft 1.17+ (ping command added in this version)" -ForegroundColor White
Write-Host "  - You need to be OP/admin to use /ping on other players" -ForegroundColor White
Write-Host ""

Write-Host "=== Quick Test ===" -ForegroundColor Cyan
Write-Host "To test if ping works, go to your server console and type:" -ForegroundColor White
Write-Host "  list" -ForegroundColor Green
Write-Host ""
Write-Host "This will show all connected players." -ForegroundColor Gray

