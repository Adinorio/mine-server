# Test playit.gg Connection
Write-Host "=== Testing playit.gg Connection ===" -ForegroundColor Cyan
Write-Host ""

# Check if server is running
Write-Host "1. Checking if Minecraft server is running..." -ForegroundColor Yellow
$serverRunning = Test-NetConnection -ComputerName localhost -Port 25565 -InformationLevel Quiet -WarningAction SilentlyContinue
if ($serverRunning) {
    Write-Host "   Server is running on localhost:25565" -ForegroundColor Green
} else {
    Write-Host "   Server is NOT running!" -ForegroundColor Red
    Write-Host "   Please start the server with: .\start-server.ps1" -ForegroundColor Yellow
    exit 1
}

# Check playit.gg connection
Write-Host ""
Write-Host "2. Testing playit.gg tunnel..." -ForegroundColor Yellow
$playitIP = "24.ip.gl.ply.gg"
$playitPort = 33308

Write-Host "   Testing connection to: $playitIP : $playitPort" -ForegroundColor White
$playitTest = Test-NetConnection -ComputerName $playitIP -Port $playitPort -InformationLevel Detailed -WarningAction SilentlyContinue

if ($playitTest.TcpTestSucceeded) {
    Write-Host "   playit.gg tunnel is working!" -ForegroundColor Green
} else {
    Write-Host "   playit.gg tunnel is NOT working!" -ForegroundColor Red
    Write-Host ""
    Write-Host "   Troubleshooting steps:" -ForegroundColor Yellow
    Write-Host "   1. Make sure playit.gg application is running" -ForegroundColor White
    Write-Host "   2. Check playit.gg shows: tunnel running, 1 tunnels registered" -ForegroundColor White
    Write-Host "   3. Restart playit.gg if needed" -ForegroundColor White
    Write-Host "   4. Verify tunnel is active in playit.gg dashboard" -ForegroundColor White
}

# Check domain resolution
Write-Host ""
Write-Host "3. Testing domain resolution..." -ForegroundColor Yellow
$domain = "comes-considers.gl.joinmc.link"
try {
    $dnsResult = Resolve-DnsName -Name $domain -ErrorAction Stop
    Write-Host "   Domain resolves to: $($dnsResult[0].IPAddress)" -ForegroundColor Green
} catch {
    Write-Host "   Domain does not resolve!" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Connection Test Complete ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "To connect:" -ForegroundColor Yellow
Write-Host "  - Local: localhost or 127.0.0.1" -ForegroundColor White
$playitAddress = $playitIP + ":" + $playitPort
Write-Host "  - Online: $playitAddress" -ForegroundColor White
Write-Host "  - Domain: comes-considers.gl.joinmc.link" -ForegroundColor White
