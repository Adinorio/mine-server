# GeyserMC Setup Script
# Enables Bedrock (PE) players to join your Java Edition server

Write-Host "=== GeyserMC Setup for Bedrock Crossplay ===" -ForegroundColor Cyan
Write-Host ""

# Check if Java is available
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

if (-not $javaPath) {
    $javaPath = "java"
}

# Create geyser directory
Write-Host "Creating Geyser directory..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path "geyser" | Out-Null

$geyserJar = "geyser\geyser.jar"

# Download GeyserMC standalone
if (-not (Test-Path $geyserJar)) {
    Write-Host ""
    Write-Host "Downloading GeyserMC..." -ForegroundColor Yellow
    
    try {
        # Download latest Geyser standalone (Windows)
        # Using direct download URL for latest standalone build
        $downloadUrl = "https://download.geysermc.org/v2/projects/geyser/versions/latest/builds/latest/downloads/standalone"
        
        Write-Host "Downloading GeyserMC from official source..." -ForegroundColor Cyan
        Invoke-WebRequest -Uri $downloadUrl -OutFile $geyserJar -UseBasicParsing
        
        Write-Host "GeyserMC downloaded successfully!" -ForegroundColor Green
    } catch {
        Write-Host "ERROR: Failed to download GeyserMC automatically!" -ForegroundColor Red
        Write-Host "Error: $_" -ForegroundColor Red
        Write-Host ""
        Write-Host "Please manually download GeyserMC:" -ForegroundColor Yellow
        Write-Host "1. Go to: https://geysermc.org/download" -ForegroundColor White
        Write-Host "2. Download 'Geyser Standalone' for your OS" -ForegroundColor White
        Write-Host "3. Extract and place geyser.jar as: $geyserJar" -ForegroundColor White
        Write-Host ""
        exit 1
    }
} else {
    Write-Host "GeyserMC already exists, skipping download." -ForegroundColor Green
}

# Generate Geyser config by running it once
Write-Host ""
Write-Host "Generating Geyser configuration files..." -ForegroundColor Yellow
Write-Host "This will start Geyser briefly to create config files..." -ForegroundColor Yellow

$geyserConfig = "geyser\config.yml"
if (-not (Test-Path $geyserConfig)) {
    Push-Location "geyser"
    
    # Start Geyser briefly to generate config (it will fail to connect but that's OK)
    $process = Start-Process -FilePath $javaPath -ArgumentList "-jar", "geyser.jar" -PassThru -NoNewWindow -RedirectStandardOutput "geyser-output.log" -RedirectStandardError "geyser-error.log"
    Start-Sleep -Seconds 5
    Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    
    Pop-Location
    Start-Sleep -Seconds 1
}

# Check if config was generated
if (Test-Path $geyserConfig) {
    Write-Host "Configuration file generated!" -ForegroundColor Green
    Write-Host ""
    Write-Host "IMPORTANT: You need to edit geyser\config.yml and set:" -ForegroundColor Yellow
    Write-Host "  remote.address: '127.0.0.1'" -ForegroundColor White
    Write-Host "  remote.port: 25565" -ForegroundColor White
    Write-Host "  remote.auth-type: 'offline'" -ForegroundColor White
    Write-Host "  bedrock.address: '0.0.0.0'" -ForegroundColor White
    Write-Host "  bedrock.port: 19132" -ForegroundColor White
    Write-Host ""
    Write-Host "Or run Geyser once and it will prompt you for these settings." -ForegroundColor Cyan
} else {
    Write-Host "WARNING: Config file not generated automatically." -ForegroundColor Yellow
    Write-Host "Run Geyser once manually to generate the config file." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Setup Complete! ===" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Start your Minecraft server: .\start-server.ps1" -ForegroundColor White
Write-Host "2. Start Geyser proxy: .\start-geyser.ps1" -ForegroundColor White
Write-Host "3. Bedrock players connect to: YOUR_IP:19132" -ForegroundColor White
Write-Host ""
Write-Host "See BEDROCK-CROSSPLAY.md for detailed connection instructions!" -ForegroundColor Yellow
Write-Host ""

