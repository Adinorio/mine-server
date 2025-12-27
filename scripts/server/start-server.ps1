# Minecraft Server Start Script
# Compatible with TLauncher and Minecraft 1.21.11

$ErrorActionPreference = "Stop"

# Configuration
$ServerDir = "server"
$ServerJar = "$ServerDir\server.jar"
$MinMemory = "5G"
$MaxMemory = "8G"

# Check if server jar exists
if (-not (Test-Path $ServerJar)) {
    Write-Host "ERROR: Server jar not found at $ServerJar" -ForegroundColor Red
    Write-Host "Please run setup-server.ps1 first!" -ForegroundColor Yellow
    exit 1
}

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

# Check Java version (need 21+)
# Java -version outputs to stderr (normal behavior), so temporarily allow errors
$oldErrorAction = $ErrorActionPreference
$ErrorActionPreference = "Continue"
$javaOutput = & $javaPath -version 2>&1
$ErrorActionPreference = $oldErrorAction

# Check if Java command was not found
if ($javaOutput -is [System.Management.Automation.ErrorRecord] -and $javaOutput.FullyQualifiedErrorId -like "*CommandNotFoundException*") {
    Write-Host "ERROR: Java is not installed or not in PATH!" -ForegroundColor Red
    Write-Host "Please install Java 21 or later from: https://adoptium.net/" -ForegroundColor Yellow
    exit 1
}

$javaVersionLine = $javaOutput | Select-Object -First 1
$majorVersion = 0
if ($javaVersionLine -match '"1\.(\d+)') {
    # Java 8 format: "1.8.0_401"
    $majorVersion = 1  # Treat as old version
} elseif ($javaVersionLine -match '"(\d+)\.') {
    # Java 9+ format: "21.0.1" or "17.0.1"
    $majorVersion = [int]$matches[1]
}

if ($majorVersion -lt 21) {
    Write-Host "ERROR: Java version $majorVersion is too old!" -ForegroundColor Red
    Write-Host "Minecraft 1.21.11 requires Java 21 or later." -ForegroundColor Yellow
    Write-Host "Please install Java 21 from: https://adoptium.net/" -ForegroundColor Yellow
    exit 1
}

Write-Host "=== Starting Minecraft Server ===" -ForegroundColor Cyan
Write-Host "Server Version: 1.21.11" -ForegroundColor White
Write-Host "Java: $javaVersionLine" -ForegroundColor White
if ($javaPath -ne "java") {
    Write-Host "Using Java from: $javaPath" -ForegroundColor Cyan
}
Write-Host "Memory: $MinMemory - $MaxMemory" -ForegroundColor White
Write-Host ""

# Get local IP address (prefer WiFi/Ethernet over virtual adapters)
$localIP = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { 
    ($_.IPAddress -like "192.168.*" -or $_.IPAddress -like "10.*") -and
    $_.InterfaceAlias -notlike "*Virtual*" -and
    $_.InterfaceAlias -notlike "*Hyper-V*" -and
    $_.InterfaceAlias -notlike "*VMware*"
} | Sort-Object InterfaceIndex | Select-Object -First 1).IPAddress
if ($localIP) {
    Write-Host "Local IP: $localIP" -ForegroundColor Green
} else {
    # Fallback to any 192.168.x.x
    $localIP = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.IPAddress -like "192.168.*" } | Select-Object -First 1).IPAddress
    if ($localIP) {
        Write-Host "Local IP: $localIP" -ForegroundColor Yellow
    }
}

# Get public IP (if possible)
try {
    $publicIP = (Invoke-WebRequest -Uri "https://api.ipify.org" -UseBasicParsing -TimeoutSec 3).Content
    Write-Host "Public IP: $publicIP" -ForegroundColor Green
} catch {
    Write-Host "Could not determine public IP" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Server starting... Press Ctrl+C to stop" -ForegroundColor Yellow
Write-Host ""

# Change to server directory
Set-Location $ServerDir

# Start the server with MAXIMUM performance optimizations
# Build arguments array with proper variable expansion
$javaArgs = @(
    "-Xms$MinMemory",
    "-Xmx$MaxMemory",
    "-XX:+UseG1GC",
    "-XX:+ParallelRefProcEnabled",
    "-XX:MaxGCPauseMillis=100",
    "-XX:+UnlockExperimentalVMOptions",
    "-XX:+DisableExplicitGC",
    "-XX:+AlwaysPreTouch",
    "-XX:G1NewSizePercent=40",
    "-XX:G1MaxNewSizePercent=50",
    "-XX:G1HeapRegionSize=16M",
    "-XX:G1ReservePercent=15",
    "-XX:G1HeapWastePercent=5",
    "-XX:G1MixedGCCountTarget=8",
    "-XX:InitiatingHeapOccupancyPercent=10",
    "-XX:G1MixedGCLiveThresholdPercent=95",
    "-XX:G1RSetUpdatingPauseTimePercent=5",
    "-XX:SurvivorRatio=32",
    "-XX:+PerfDisableSharedMem",
    "-XX:MaxTenuringThreshold=1",
    "-XX:+UseStringDeduplication",
    "-XX:+OptimizeStringConcat",
    "-XX:+UseCompressedOops",
    "-XX:+UseFastUnorderedTimeStamps",
    "-Dfile.encoding=UTF-8",
    "-Dusing.aikars.flags=https://mcflags.emc.gs",
    "-Daikars.new.flags=true",
    "-jar", "server.jar", "nogui"
)
& $javaPath $javaArgs

# Return to original directory
Set-Location ..

Write-Host ""
Write-Host "Server stopped." -ForegroundColor Yellow

