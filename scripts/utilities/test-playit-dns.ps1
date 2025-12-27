# Test playit.gg DNS Resolution
param(
    [string]$Domain = "",
    [string]$IP = "",
    [string]$Port = ""
)

Write-Host "=== Testing playit.gg Connection ===" -ForegroundColor Cyan
Write-Host ""

if ($Domain -eq "" -and $IP -eq "") {
    Write-Host "Usage: .\test-playit-dns.ps1 -Domain 'your-domain.gl.joinmc.link' -IP 'x.x.x.x' -Port 'xxxxx'" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Or provide the domain/IP from playit.gg dashboard" -ForegroundColor White
    exit 1
}

# Test domain if provided
if ($Domain -ne "") {
    Write-Host "1. Testing Domain: $Domain" -ForegroundColor Yellow
    try {
        $dnsResult = Resolve-DnsName -Name $Domain -Type A -ErrorAction Stop
        $ipAddress = $dnsResult[0].IPAddress
        Write-Host "   Domain resolves to: $ipAddress" -ForegroundColor Green
        
        # Test connection
        if ($Port -ne "") {
            Write-Host "   Testing connection to: $Domain`:$Port" -ForegroundColor White
            $test = Test-NetConnection -ComputerName $Domain -Port $Port -InformationLevel Quiet -WarningAction SilentlyContinue
            if ($test) {
                Write-Host "   Connection successful!" -ForegroundColor Green
            } else {
                Write-Host "   Connection failed" -ForegroundColor Red
            }
        }
    } catch {
        Write-Host "   Domain does NOT resolve!" -ForegroundColor Red
        Write-Host "   Error: $_" -ForegroundColor Yellow
    }
}

# Test IP if provided
if ($IP -ne "" -and $Port -ne "") {
    Write-Host ""
    Write-Host "2. Testing IP Address: $IP`:$Port" -ForegroundColor Yellow
    $test = Test-NetConnection -ComputerName $IP -Port $Port -InformationLevel Detailed -WarningAction SilentlyContinue
    if ($test.TcpTestSucceeded) {
        Write-Host "   Connection successful!" -ForegroundColor Green
    } else {
        Write-Host "   Connection failed: $($test.TcpState)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== Recommendations ===" -ForegroundColor Cyan
Write-Host "If domain doesn't resolve, use the IP address instead" -ForegroundColor White
Write-Host "Format: IP:Port (e.g., 147.185.221.24:33308)" -ForegroundColor White











