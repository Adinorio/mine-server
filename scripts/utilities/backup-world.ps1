# Minecraft Server World Backup Script

$ServerDir = "server"
$WorldDir = "$ServerDir\world"
$BackupDir = "backups"

# Check if world exists
if (-not (Test-Path $WorldDir)) {
    Write-Host "ERROR: World directory not found at $WorldDir" -ForegroundColor Red
    Write-Host "Make sure the server has been started at least once." -ForegroundColor Yellow
    exit 1
}

# Create backup directory if it doesn't exist
if (-not (Test-Path $BackupDir)) {
    New-Item -ItemType Directory -Force -Path $BackupDir | Out-Null
}

# Generate backup name with timestamp
$timestamp = Get-Date -Format "yyyy-MM-dd-HHmmss"
$backupName = "world-$timestamp"
$backupPath = "$BackupDir\$backupName"

Write-Host "=== Creating World Backup ===" -ForegroundColor Cyan
Write-Host "Source: $WorldDir" -ForegroundColor White
Write-Host "Destination: $backupPath" -ForegroundColor White
Write-Host ""

# Copy world directory (skip session.lock if locked by server)
Write-Host "Copying world files..." -ForegroundColor Yellow
try {
    Copy-Item -Path $WorldDir -Destination $backupPath -Recurse -Force -ErrorAction Stop
} catch {
    # If session.lock is locked (server is running), that's OK - continue
    if ($_.Exception.Message -like "*session.lock*" -or $_.Exception.Message -like "*locked*") {
        Write-Host "Note: session.lock is locked (server is running) - this is normal" -ForegroundColor Yellow
        # Try copying again, skipping locked files
        Get-ChildItem -Path $WorldDir -Recurse | ForEach-Object {
            $destPath = $_.FullName.Replace($WorldDir, $backupPath)
            $destDir = Split-Path $destPath -Parent
            if (-not (Test-Path $destDir)) {
                New-Item -ItemType Directory -Force -Path $destDir | Out-Null
            }
            try {
                Copy-Item -Path $_.FullName -Destination $destPath -Force -ErrorAction Stop
            } catch {
                # Skip locked files (like session.lock)
                if ($_.Exception.Message -notlike "*locked*") {
                    Write-Warning "Could not copy: $($_.Name)"
                }
            }
        }
    } else {
        throw
    }
}

Write-Host ""
Write-Host "Backup created successfully!" -ForegroundColor Green
Write-Host "Location: $backupPath" -ForegroundColor White

# Optional: Keep only last 10 backups
$backups = Get-ChildItem -Path $BackupDir -Directory | Sort-Object LastWriteTime -Descending
if ($backups.Count -gt 10) {
    Write-Host ""
    Write-Host "Removing old backups (keeping last 10)..." -ForegroundColor Yellow
    $backups | Select-Object -Skip 10 | Remove-Item -Recurse -Force
    Write-Host "Old backups removed." -ForegroundColor Green
}








