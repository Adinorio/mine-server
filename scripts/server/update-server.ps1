# Minecraft Server Update Script
# Updates your server to a specified version

param(
    [Parameter(Mandatory=$false)]
    [string]$Version = "latest"
)

Write-Host "=== Minecraft Server Update ===" -ForegroundColor Cyan
Write-Host ""

# Check if server jar exists
$serverJar = "server\server.jar"
if (-not (Test-Path $serverJar)) {
    Write-Host "ERROR: Server jar not found at $serverJar" -ForegroundColor Red
    Write-Host "Please run setup-server.ps1 first!" -ForegroundColor Yellow
    exit 1
}

# Get current version info
Write-Host "Checking available versions..." -ForegroundColor Yellow
try {
    $versionManifest = Invoke-RestMethod -Uri "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json" -UseBasicParsing
    
    if ($Version -eq "latest") {
        # Get latest release version
        $targetVersion = $versionManifest.latest.release
        Write-Host "Latest release version: $targetVersion" -ForegroundColor Green
    } else {
        # Check if specified version exists
        $targetVersion = $Version
        $versionExists = $versionManifest.versions | Where-Object { $_.id -eq $targetVersion }
        if (-not $versionExists) {
            Write-Host "ERROR: Version $targetVersion not found!" -ForegroundColor Red
            Write-Host ""
            Write-Host "Available latest versions:" -ForegroundColor Yellow
            $versionManifest.versions | Select-Object -First 10 | ForEach-Object {
                Write-Host "  - $($_.id)" -ForegroundColor White
            }
            exit 1
        }
    }
    
    Write-Host "Target version: $targetVersion" -ForegroundColor Cyan
    Write-Host ""
    
    # Confirm update
    $confirm = Read-Host "Update server to version $targetVersion? (yes/no)"
    if ($confirm -ne "yes" -and $confirm -ne "y") {
        Write-Host "Update cancelled." -ForegroundColor Yellow
        exit 0
    }
    
    # Backup current server jar
    Write-Host ""
    Write-Host "Backing up current server jar..." -ForegroundColor Yellow
    $backupDir = "server\versions"
    New-Item -ItemType Directory -Force -Path $backupDir | Out-Null
    $backupFile = "$backupDir\server-$(Get-Date -Format 'yyyy-MM-dd-HHmmss').jar"
    Copy-Item -Path $serverJar -Destination $backupFile
    Write-Host "Backup saved to: $backupFile" -ForegroundColor Green
    
    # Get download URL for target version
    Write-Host ""
    Write-Host "Fetching download URL for $targetVersion..." -ForegroundColor Yellow
    $versionInfo = $versionManifest.versions | Where-Object { $_.id -eq $targetVersion } | Select-Object -First 1
    
    if ($versionInfo) {
        $versionDetails = Invoke-RestMethod -Uri $versionInfo.url -UseBasicParsing
        $serverUrl = $versionDetails.downloads.server.url
        
        Write-Host "Downloading $targetVersion..." -ForegroundColor Yellow
        Write-Host "URL: $serverUrl" -ForegroundColor Cyan
        
        # Download new server jar
        Invoke-WebRequest -Uri $serverUrl -OutFile $serverJar -UseBasicParsing
        
        Write-Host ""
        Write-Host "Server updated successfully to $targetVersion!" -ForegroundColor Green
        Write-Host ""
        Write-Host "IMPORTANT:" -ForegroundColor Yellow
        Write-Host "1. Make sure your world is backed up!" -ForegroundColor White
        Write-Host "2. Test the new version before playing" -ForegroundColor White
        Write-Host "3. If something goes wrong, restore from backup" -ForegroundColor White
        Write-Host "4. Backup location: $backupFile" -ForegroundColor White
        Write-Host ""
        Write-Host "Note: First start with new version may take longer as it updates world format." -ForegroundColor Cyan
        
    } else {
        Write-Host "ERROR: Could not find version details for $targetVersion" -ForegroundColor Red
        exit 1
    }
    
} catch {
    Write-Host "ERROR: Failed to update server!" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "You can manually update:" -ForegroundColor Yellow
    Write-Host "1. Go to: https://www.minecraft.net/en-us/download/server" -ForegroundColor White
    Write-Host "2. Or: https://mcversions.net/" -ForegroundColor White
    Write-Host "3. Download the server.jar for your desired version" -ForegroundColor White
    Write-Host "4. Replace server\server.jar (backup the old one first!)" -ForegroundColor White
    exit 1
}

Write-Host ""



