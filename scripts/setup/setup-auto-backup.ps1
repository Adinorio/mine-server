# Setup Automatic Backups
# This creates a Windows Scheduled Task to backup your world automatically

Write-Host "=== Setting Up Automatic Backups ===" -ForegroundColor Cyan
Write-Host ""

$scriptPath = Join-Path $PSScriptRoot "auto-backup.ps1"
$taskName = "MinecraftServerAutoBackup"

# Check if task already exists
$existingTask = Get-ScheduledTask -TaskName $taskName -ErrorAction SilentlyContinue

if ($existingTask) {
    Write-Host "Automatic backup task already exists!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Current schedule:" -ForegroundColor Cyan
    $trigger = (Get-ScheduledTask -TaskName $taskName).Triggers
    Write-Host "  Frequency: Every $($trigger.Repetition.Interval.Minutes) minutes" -ForegroundColor White
    Write-Host ""
    $response = Read-Host "Do you want to recreate it? (y/n)"
    if ($response -ne "y") {
        Write-Host "Keeping existing task." -ForegroundColor Green
        exit 0
    }
    Unregister-ScheduledTask -TaskName $taskName -Confirm:$false
}

Write-Host "Creating scheduled task..." -ForegroundColor Yellow

# Create action (run PowerShell script)
$action = New-ScheduledTaskAction -Execute "PowerShell.exe" `
    -Argument "-ExecutionPolicy Bypass -File `"$scriptPath`""

# Create trigger (every 30 minutes)
$trigger = New-ScheduledTaskTrigger -Once -At (Get-Date) -RepetitionInterval (New-TimeSpan -Minutes 30) -RepetitionDuration (New-TimeSpan -Days 365)

# Create settings
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -StartWhenAvailable

# Register the task
Register-ScheduledTask -TaskName $taskName -Action $action -Trigger $trigger -Settings $settings -Description "Automatically backup Minecraft server world every 30 minutes" | Out-Null

Write-Host ""
Write-Host "✅ Automatic backup task created!" -ForegroundColor Green
Write-Host ""
Write-Host "Backup schedule:" -ForegroundColor Cyan
Write-Host "  • Frequency: Every 30 minutes" -ForegroundColor White
Write-Host "  • Location: backups\ folder" -ForegroundColor White
Write-Host "  • Keeps: Last 20 backups" -ForegroundColor White
Write-Host ""
Write-Host "To change frequency, run this script again or edit in Task Scheduler." -ForegroundColor Yellow
Write-Host "Task name: $taskName" -ForegroundColor Gray





