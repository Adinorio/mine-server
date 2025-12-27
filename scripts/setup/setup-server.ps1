# Minecraft Server Setup Script for Windows
# Compatible with TLauncher and Minecraft 1.21.11

Write-Host "=== Minecraft Server Setup ===" -ForegroundColor Cyan
Write-Host ""

# Check if Java is installed and version
Write-Host "Checking Java installation..." -ForegroundColor Yellow

# Try to find Java 21+ in common locations
$javaPath = $null
$javaLocations = @(
    "C:\Program Files\Java\jdk-25\bin\java.exe",
    "C:\Program Files\Java\jdk-21\bin\java.exe",
    "C:\Program Files\Eclipse Adoptium\jdk-25*\bin\java.exe",
    "C:\Program Files\Eclipse Adoptium\jdk-21*\bin\java.exe"
)

foreach ($location in $javaLocations) {
    $resolved = Resolve-Path $location -ErrorAction SilentlyContinue
    if ($resolved) {
        $javaPath = $resolved.Path
        break
    }
}

# If not found in common locations, try PATH
if (-not $javaPath) {
    $javaPath = "java"
}

# Check Java version
$javaOutput = & $javaPath -version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Java is not installed or not in PATH!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install Java 21 or later:" -ForegroundColor Yellow
    Write-Host "1. Go to: https://adoptium.net/temurin/releases/" -ForegroundColor White
    Write-Host "2. Download 'JDK 21' for Windows x64" -ForegroundColor White
    Write-Host "3. Install it and restart PowerShell" -ForegroundColor White
    exit 1
}

# Check Java version (need 21+)
$javaVersionLine = $javaOutput | Select-Object -First 1
Write-Host "Java found: $javaVersionLine" -ForegroundColor White

# Extract version number (handle both "1.8" and "21" formats)
$majorVersion = 0
if ($javaVersionLine -match '"1\.(\d+)') {
    # Java 8 format: "1.8.0_401"
    $minorVersion = [int]$matches[1]
    if ($minorVersion -lt 21) {
        $majorVersion = 1  # Treat as old version
    }
} elseif ($javaVersionLine -match '"(\d+)\.') {
    # Java 9+ format: "21.0.1" or "17.0.1"
    $majorVersion = [int]$matches[1]
}

if ($majorVersion -lt 21) {
    Write-Host ""
    Write-Host "ERROR: Java version $majorVersion is too old!" -ForegroundColor Red
    Write-Host "Minecraft 1.21.11 requires Java 21 or later." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Please install Java 21:" -ForegroundColor Yellow
    Write-Host "1. Go to: https://adoptium.net/temurin/releases/" -ForegroundColor White
    Write-Host "2. Download 'JDK 21' for Windows x64" -ForegroundColor White
    Write-Host "3. Install it (you can keep Java 8 installed)" -ForegroundColor White
    Write-Host "4. Restart PowerShell and run this script again" -ForegroundColor White
    Write-Host ""
    Write-Host "Note: If you have multiple Java versions, you may need to update PATH" -ForegroundColor Yellow
    Write-Host "      or specify Java 21 path in start-server.ps1" -ForegroundColor Yellow
    exit 1
} else {
    Write-Host "Java version $majorVersion is compatible!" -ForegroundColor Green
}

if ($majorVersion -eq 0) {
    Write-Host "Warning: Could not determine Java version, continuing anyway..." -ForegroundColor Yellow
}

# Create server directory structure
Write-Host ""
Write-Host "Creating server directory structure..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path "server" | Out-Null
New-Item -ItemType Directory -Force -Path "server\worlds" | Out-Null
New-Item -ItemType Directory -Force -Path "server\plugins" | Out-Null
New-Item -ItemType Directory -Force -Path "server\logs" | Out-Null

# Download Minecraft server jar (1.21.11)
Write-Host ""
Write-Host "Downloading Minecraft Server 1.21.11..." -ForegroundColor Yellow
$serverJar = "server\server.jar"

if (-not (Test-Path $serverJar)) {
    try {
        # Try to get the correct download URL from version manifest
        Write-Host "Fetching server download URL..." -ForegroundColor Yellow
        $versionManifest = Invoke-RestMethod -Uri "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json" -UseBasicParsing
        $versionInfo = $versionManifest.versions | Where-Object { $_.id -eq "1.21.11" } | Select-Object -First 1
        
        if ($versionInfo) {
            $versionDetails = Invoke-RestMethod -Uri $versionInfo.url -UseBasicParsing
            $serverUrl = $versionDetails.downloads.server.url
            
            Write-Host "Downloading from: $serverUrl" -ForegroundColor Cyan
            Invoke-WebRequest -Uri $serverUrl -OutFile $serverJar -UseBasicParsing
            Write-Host "Server jar downloaded successfully!" -ForegroundColor Green
        } else {
            throw "Version 1.21.11 not found in manifest"
        }
    } catch {
        Write-Host "ERROR: Failed to download server jar automatically!" -ForegroundColor Red
        Write-Host "Error: $_" -ForegroundColor Red
        Write-Host ""
        Write-Host "Please manually download Minecraft Server 1.21.11:" -ForegroundColor Yellow
        Write-Host "1. Go to: https://www.minecraft.net/en-us/download/server" -ForegroundColor White
        Write-Host "2. Or visit: https://mcversions.net/download/1.21.11" -ForegroundColor White
        Write-Host "3. Download the server.jar file" -ForegroundColor White
        Write-Host "4. Place it as: $serverJar" -ForegroundColor White
        Write-Host ""
        Write-Host "After downloading, run this script again to continue setup." -ForegroundColor Yellow
        exit 1
    }
} else {
    Write-Host "Server jar already exists, skipping download." -ForegroundColor Green
}

# Create eula.txt
Write-Host ""
Write-Host "Creating EULA agreement..." -ForegroundColor Yellow
$eulaPath = "server\eula.txt"
if (-not (Test-Path $eulaPath)) {
    Set-Content -Path $eulaPath -Value "eula=true"
    Write-Host "EULA accepted automatically." -ForegroundColor Green
} else {
    Write-Host "EULA already exists." -ForegroundColor Green
}

# Copy server.properties if it doesn't exist
Write-Host ""
Write-Host "Setting up server configuration..." -ForegroundColor Yellow
if (-not (Test-Path "server\server.properties")) {
    Copy-Item -Path "server.properties" -Destination "server\server.properties" -ErrorAction SilentlyContinue
    Write-Host "Server properties configured." -ForegroundColor Green
} else {
    Write-Host "Server properties already exist." -ForegroundColor Green
}

# First run to generate world
Write-Host ""
Write-Host "Starting server for first time to generate world..." -ForegroundColor Yellow
Write-Host "This may take a few minutes. Please wait..." -ForegroundColor Yellow

$startScript = Get-Content "start-server.ps1" -Raw
$process = Start-Process powershell -ArgumentList "-NoProfile", "-Command", $startScript -PassThru -WindowStyle Minimized

# Wait a bit for server to start
Start-Sleep -Seconds 10

# Stop the server
Write-Host "Stopping initial server start..." -ForegroundColor Yellow
Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

Write-Host ""
Write-Host "=== Setup Complete! ===" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Review server\server.properties and adjust settings as needed" -ForegroundColor White
Write-Host "2. Configure port forwarding on your router (port 25565)" -ForegroundColor White
Write-Host "3. Or use a tunneling service like playit.gg or ngrok" -ForegroundColor White
Write-Host "4. Run start-server.ps1 to start your server" -ForegroundColor White
Write-Host ""
Write-Host "See README.md for detailed instructions!" -ForegroundColor Yellow

