# Get Your Local IP Address for Minecraft Server
Write-Host "=== Your Minecraft Server Local IP Address ===" -ForegroundColor Cyan
Write-Host ""

# Get WiFi/local network IP
$localIPs = Get-NetIPAddress -AddressFamily IPv4 | Where-Object { 
    $_.IPAddress -like "192.168.*" -or $_.IPAddress -like "10.*" 
} | Select-Object IPAddress, InterfaceAlias

if ($localIPs) {
    Write-Host "Your server address for friends on the same WiFi:" -ForegroundColor Yellow
    Write-Host ""
    foreach ($ip in $localIPs) {
        Write-Host "  $($ip.IPAddress):25565" -ForegroundColor Green
        Write-Host "    (Network: $($ip.InterfaceAlias))" -ForegroundColor Gray
    }
    Write-Host ""
    Write-Host "Note: This IP might change if you:" -ForegroundColor Yellow
    Write-Host "  - Disconnect/reconnect to WiFi" -ForegroundColor White
    Write-Host "  - Restart your laptop" -ForegroundColor White
    Write-Host "  - Connect to a different network" -ForegroundColor White
    Write-Host ""
    Write-Host "To find your IP again, run: .\get-my-ip.ps1" -ForegroundColor Cyan
} else {
    Write-Host "No local network IP found. Make sure you're connected to WiFi." -ForegroundColor Red
}

Write-Host ""
Write-Host "=== World/Save Data ===" -ForegroundColor Cyan
if (Test-Path "server\world") {
    Write-Host "Your world is saved in: server\world" -ForegroundColor Green
    Write-Host "This persists even when you stop the server!" -ForegroundColor Green
} else {
    Write-Host "World folder not found yet. It will be created when you first start the server." -ForegroundColor Yellow
}











