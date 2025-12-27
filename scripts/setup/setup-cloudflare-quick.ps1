# Quick Cloudflare Tunnel Setup
Write-Host "=== Cloudflare Tunnel Quick Setup ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "This creates a temporary tunnel (URL changes on restart)" -ForegroundColor Yellow
Write-Host "For permanent address, see: CLOUDFLARE-TUNNEL-SETUP.md" -ForegroundColor White
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

# Check for cloudflared
Write-Host "Looking for cloudflared..." -ForegroundColor Yellow
$cloudflaredPath = $null

# Check common locations
$commonPaths = @(
    ".\cloudfared.exe",
    ".\cloudflared.exe",
    ".\cloudflared\cloudflared.exe",
    "C:\cloudflared\cloudflared.exe",
    "$env:USERPROFILE\cloudflared\cloudflared.exe",
    "$env:USERPROFILE\Downloads\cloudflared.exe"
)

foreach ($path in $commonPaths) {
    if (Test-Path $path) {
        $cloudflaredPath = (Resolve-Path $path).Path
        Write-Host "Found cloudflared at: $cloudflaredPath" -ForegroundColor Green
        break
    }
}

if (-not $cloudflaredPath) {
    Write-Host "cloudflared not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "To download:" -ForegroundColor Yellow
    Write-Host "1. Go to: https://github.com/cloudflare/cloudflared/releases/latest" -ForegroundColor White
    Write-Host "2. Download: cloudflared-windows-amd64.exe" -ForegroundColor White
    Write-Host "3. Rename to: cloudflared.exe" -ForegroundColor White
    Write-Host "4. Place in this folder or C:\cloudflared\" -ForegroundColor White
    Write-Host ""
    Write-Host "Or use direct link:" -ForegroundColor Yellow
    Write-Host "https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-windows-amd64.exe" -ForegroundColor Cyan
    exit 1
}

Write-Host ""
Write-Host "Starting Cloudflare Tunnel..." -ForegroundColor Cyan
Write-Host "This will create a temporary URL (changes on restart)" -ForegroundColor Yellow
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

