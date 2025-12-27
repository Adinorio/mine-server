# Start ngrok Tunnel for Minecraft Server
# Make sure your Minecraft server is running first!

Write-Host "=== Starting ngrok Tunnel for Minecraft Server ===" -ForegroundColor Cyan
Write-Host ""

# Check if server is running
Write-Host "Checking if Minecraft server is running..." -ForegroundColor Yellow
$serverRunning = Test-NetConnection -ComputerName localhost -Port 25565 -InformationLevel Quiet -WarningAction SilentlyContinue
if (-not $serverRunning) {
    Write-Host "ERROR: Minecraft server is not running on localhost:25565!" -ForegroundColor Red
    Write-Host "Please start your server first with: .\start-server.ps1" -ForegroundColor Yellow
    exit 1
}
Write-Host "Server is running!" -ForegroundColor Green
Write-Host ""

# Find ngrok
Write-Host "Looking for ngrok..." -ForegroundColor Yellow
$ngrokPath = $null

# Check if ngrok is in PATH
$ngrokInPath = Get-Command ngrok -ErrorAction SilentlyContinue
if ($ngrokInPath) {
    $ngrokPath = $ngrokInPath.Path
    Write-Host "Found ngrok in PATH: $ngrokPath" -ForegroundColor Green
} else {
    # Check common locations
    $commonPaths = @(
        ".\ngrok-v3-stable-windows-amd64\ngrok.exe",
        ".\ngrok.exe",
        ".\ngrok\ngrok.exe",
        "C:\ngrok\ngrok.exe",
        "$env:USERPROFILE\ngrok\ngrok.exe",
        "$env:USERPROFILE\Downloads\ngrok.exe"
    )
    
    foreach ($path in $commonPaths) {
        if (Test-Path $path) {
            $ngrokPath = (Resolve-Path $path).Path
            Write-Host "Found ngrok at: $ngrokPath" -ForegroundColor Green
            break
        }
    }
}

if (-not $ngrokPath) {
    Write-Host "ERROR: ngrok not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please:" -ForegroundColor Yellow
    Write-Host "1. Download ngrok from: https://ngrok.com/download" -ForegroundColor White
    Write-Host "2. Extract ngrok.exe to a folder (e.g., C:\ngrok\)" -ForegroundColor White
    Write-Host "3. Configure your authtoken:" -ForegroundColor White
    Write-Host "   cd C:\ngrok" -ForegroundColor Gray
    Write-Host "   .\ngrok config add-authtoken YOUR_TOKEN" -ForegroundColor Gray
    Write-Host "4. Update this script with the ngrok path, or add ngrok to PATH" -ForegroundColor White
    exit 1
}

# Check if authtoken is configured (ngrok v3 uses different location)
Write-Host ""
Write-Host "Checking ngrok configuration..." -ForegroundColor Yellow
$ngrokConfig1 = "$env:USERPROFILE\.ngrok2\ngrok.yml"
$ngrokConfig2 = "$env:LOCALAPPDATA\ngrok\ngrok.yml"
$hasConfig = (Test-Path $ngrokConfig1) -or (Test-Path $ngrokConfig2)

if (-not $hasConfig) {
    Write-Host "WARNING: ngrok authtoken not configured!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Please configure your authtoken:" -ForegroundColor Yellow
    Write-Host "1. Get your token from: https://dashboard.ngrok.com/get-started/your-authtoken" -ForegroundColor White
    Write-Host "2. Run: cd ngrok-v3-stable-windows-amd64" -ForegroundColor White
    Write-Host "3. Run: .\ngrok config add-authtoken YOUR_TOKEN" -ForegroundColor White
    Write-Host ""
    $continue = Read-Host "Continue anyway? (y/n)"
    if ($continue -ne "y") {
        exit 1
    }
} else {
    Write-Host "ngrok authtoken is configured!" -ForegroundColor Green
}

# Start ngrok tunnel
Write-Host ""
Write-Host "Starting ngrok TCP tunnel on port 25565..." -ForegroundColor Cyan
Write-Host "Keep this window open while the server is running!" -ForegroundColor Yellow
Write-Host ""
Write-Host "The tunnel address will be shown below." -ForegroundColor White
Write-Host "Share that address with friends to connect!" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop ngrok" -ForegroundColor Yellow
Write-Host ""

# Start ngrok
$ngrokDir = Split-Path $ngrokPath -Parent
Set-Location $ngrokDir
& $ngrokPath tcp 25565

# Return to original directory
Set-Location $PSScriptRoot

