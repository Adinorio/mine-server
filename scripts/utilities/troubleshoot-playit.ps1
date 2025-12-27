# Troubleshoot playit.gg Connection Issues
Write-Host "=== Troubleshooting playit.gg Connection ===" -ForegroundColor Cyan
Write-Host ""

# Check server status
Write-Host "1. Server Status:" -ForegroundColor Yellow
$serverRunning = Test-NetConnection -ComputerName localhost -Port 25565 -InformationLevel Quiet -WarningAction SilentlyContinue
if ($serverRunning) {
    Write-Host "   Server is running on localhost:25565" -ForegroundColor Green
} else {
    Write-Host "   Server is NOT running!" -ForegroundColor Red
    exit 1
}

# Check playit.gg tunnel
Write-Host ""
Write-Host "2. playit.gg Tunnel Test:" -ForegroundColor Yellow
Write-Host "   Testing connection to playit.gg endpoint..." -ForegroundColor White
$playitTest = Test-NetConnection -ComputerName 147.185.221.24 -Port 33308 -InformationLevel Detailed -WarningAction SilentlyContinue

if ($playitTest.TcpTestSucceeded) {
    Write-Host "   playit.gg endpoint is reachable" -ForegroundColor Green
} else {
    Write-Host "   playit.gg endpoint is NOT reachable" -ForegroundColor Red
    Write-Host "   This might be normal - the tunnel forwards to localhost" -ForegroundColor Yellow
}

# Check if playit.gg can reach localhost
Write-Host ""
Write-Host "3. Local Connection Test:" -ForegroundColor Yellow
Write-Host "   Testing if server accepts connections from 127.0.0.1..." -ForegroundColor White
$localTest = Test-NetConnection -ComputerName 127.0.0.1 -Port 25565 -InformationLevel Detailed -WarningAction SilentlyContinue
if ($localTest.TcpTestSucceeded) {
    Write-Host "   Server accepts localhost connections" -ForegroundColor Green
} else {
    Write-Host "   Server does NOT accept localhost connections!" -ForegroundColor Red
}

# Check server properties
Write-Host ""
Write-Host "4. Server Configuration:" -ForegroundColor Yellow
if (Test-Path "server\server.properties") {
    $props = Get-Content "server\server.properties"
    $onlineMode = ($props | Select-String "^online-mode=").ToString().Split("=")[1]
    $preventProxy = ($props | Select-String "^prevent-proxy-connections=").ToString().Split("=")[1]
    $whitelist = ($props | Select-String "^white-list=").ToString().Split("=")[1]
    
    Write-Host "   online-mode: $onlineMode" -ForegroundColor White
    Write-Host "   prevent-proxy-connections: $preventProxy" -ForegroundColor White
    Write-Host "   white-list: $whitelist" -ForegroundColor White
    
    if ($preventProxy -eq "true") {
        Write-Host "   WARNING: prevent-proxy-connections is true!" -ForegroundColor Red
        Write-Host "   This might block playit.gg connections!" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=== Recommendations ===" -ForegroundColor Cyan
Write-Host "1. Make sure playit.gg is running and shows 'tunnel running'" -ForegroundColor White
Write-Host "2. Try restarting both playit.gg and the server" -ForegroundColor White
Write-Host "3. Check playit.gg dashboard to verify tunnel is active" -ForegroundColor White
Write-Host "4. Try connecting from a different network/device" -ForegroundColor White
Write-Host "5. Check if your ISP/router blocks certain connections" -ForegroundColor White











