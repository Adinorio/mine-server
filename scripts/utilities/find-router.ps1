# Find Your Router's Admin Panel
Write-Host "=== Finding Your Router ===" -ForegroundColor Cyan
Write-Host ""

# Get default gateway
$gateway = (Get-NetRoute -DestinationPrefix "0.0.0.0/0" | Where-Object { 
    $_.NextHop -like "192.168.*" -or $_.NextHop -like "10.*" 
}).NextHop | Select-Object -First 1

if (-not $gateway) {
    $gateway = (Get-NetIPConfiguration | Where-Object { 
        $_.IPv4DefaultGateway 
    }).IPv4DefaultGateway.NextHop | Select-Object -First 1
}

if ($gateway) {
    Write-Host "Your router's admin panel:" -ForegroundColor Green
    Write-Host "  http://$gateway" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Open this in your web browser to access router settings" -ForegroundColor White
    Write-Host ""
    
    # Get local IP
    $localIP = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { 
        $_.IPAddress -like "192.168.*" -and $_.InterfaceAlias -like "*Wi-Fi*" 
    }).IPAddress | Select-Object -First 1
    
    if ($localIP) {
        Write-Host "Your laptop's IP address: $localIP" -ForegroundColor Green
        Write-Host "(Use this for 'Internal IP' in port forwarding)" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "Port forwarding settings:" -ForegroundColor Yellow
    Write-Host "  External Port: 25565" -ForegroundColor White
    Write-Host "  Internal Port: 25565" -ForegroundColor White
    Write-Host "  Protocol: TCP" -ForegroundColor White
    Write-Host "  Internal IP: $localIP" -ForegroundColor White
} else {
    Write-Host "Could not find router automatically" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Try running: ipconfig" -ForegroundColor White
    Write-Host "Look for 'Default Gateway' - usually 192.168.1.1 or 192.168.0.1" -ForegroundColor White
}









