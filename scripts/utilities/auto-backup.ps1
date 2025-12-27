# Automatic Backup Script
# Run this periodically to backup your world

$ServerDir = "server"
$WorldDir = "$ServerDir\world"
$BackupDir = "backups"

# Check if world exists
if (-not (Test-Path $WorldDir)) {
    Write-Host "World not found. Server may not have started yet." -ForegroundColor Yellow
    exit 0
}

# Create backup directory if it doesn't exist
if (-not (Test-Path $BackupDir)) {
    New-Item -ItemType Directory -Force -Path $BackupDir | Out-Null
}

# Generate backup name with timestamp
$timestamp = Get-Date -Format "yyyy-MM-dd-HHmmss"
$backupName = "world-$timestamp"
$backupPath = "$BackupDir\$backupName"

Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Creating backup..." -ForegroundColor Cyan

# Copy world directory (skip session.lock if locked by server)
try {
    Copy-Item -Path $WorldDir -Destination $backupPath -Recurse -Force -ErrorAction Stop
} catch {
    # If session.lock is locked (server is running), that's OK - continue
    if ($_.Exception.Message -like "*session.lock*" -or $_.Exception.Message -like "*locked*") {
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
                # Skip locked files (like session.lock) silently
            }
        }
    }
}

if (Test-Path $backupPath) {
    Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Backup created: $backupName" -ForegroundColor Green
    
    # Keep only last 20 backups
    $backups = Get-ChildItem -Path $BackupDir -Directory | Sort-Object LastWriteTime -Descending
    if ($backups.Count -gt 20) {
        $oldBackups = $backups | Select-Object -Skip 20
        $oldBackups | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Removed $($oldBackups.Count) old backup(s)" -ForegroundColor Yellow
    }
} else {
    Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Backup failed!" -ForegroundColor Red
}

