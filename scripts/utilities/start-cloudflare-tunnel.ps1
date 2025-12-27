# Start Cloudflare Tunnel for Minecraft Server
Write-Host "=== Starting Cloudflare Tunnel ===" -ForegroundColor Cyan
Write-Host ""

# Check if server is running
Write-Host "Checking if Minecraft server is running..." -ForegroundColor Yellow
$serverRunning = Test-NetConnection -ComputerName localhost -Port 25565 -InformationLevel Quiet -WarningAction SilentlyContinue
if (-not $serverRunning) {
    Write-Host "ERROR: Minecraft server is not running!" -ForegroundColor Red
    Write-Host "Please start your server first: .\start-server.ps1" -ForegroundColor Yellow
    exit 1
}
Write-Host "Server is running!" -ForegroundColor Green
Write-Host ""

# Find cloudflared
Write-Host "Looking for cloudflared..." -ForegroundColor Yellow
$cloudflaredPath = $null

$commonPaths = @(
    ".\cloudfared.exe",
    ".\cloudflared.exe",
    ".\cloudflared\cloudflared.exe",
    "C:\cloudflared\cloudflared.exe"
)

foreach ($path in $commonPaths) {
    if (Test-Path $path) {
        $cloudflaredPath = (Resolve-Path $path).Path
        Write-Host "Found cloudflared at: $cloudflaredPath" -ForegroundColor Green
        break
    }
}

if (-not $cloudflaredPath) {
    Write-Host "ERROR: cloudflared.exe not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please download cloudflared:" -ForegroundColor Yellow
    Write-Host "https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-windows-amd64.exe" -ForegroundColor Cyan
    Write-Host "Rename to cloudflared.exe and place in this folder" -ForegroundColor White
    exit 1
}

Write-Host ""
Write-Host "Starting Cloudflare Tunnel..." -ForegroundColor Cyan
Write-Host "This creates a temporary URL (changes on restart)" -ForegroundColor Yellow
Write-Host "Keep this window open while friends are playing!" -ForegroundColor Yellow
Write-Host ""
Write-Host "The tunnel address will be shown below." -ForegroundColor White
Write-Host "Share that address with friends to connect!" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow
Write-Host ""

# Start tunnel
$cloudflaredDir = Split-Path $cloudflaredPath -Parent
Set-Location $cloudflaredDir
& $cloudflaredPath tunnel --url tcp://localhost:25565

# Return to original directory
Set-Location $PSScriptRoot









