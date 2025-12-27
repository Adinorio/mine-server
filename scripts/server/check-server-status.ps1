# Server Status Check Script
# Checks if everything is running properly

Write-Host "=== Minecraft Server Status Check ===" -ForegroundColor Cyan
Write-Host ""

$allGood = $true

# Check 1: Java Server
Write-Host "1. Checking Java Server..." -ForegroundColor Yellow
try {
    $serverConnection = Test-NetConnection -ComputerName "127.0.0.1" -Port 25565 -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
    if ($serverConnection.TcpTestSucceeded) {
        Write-Host "   ✅ Java server is RUNNING on port 25565" -ForegroundColor Green
    } else {
        Write-Host "   ❌ Java server is NOT running" -ForegroundColor Red
        Write-Host "      → Start with: .\start-server.ps1" -ForegroundColor Yellow
        $allGood = $false
    }
} catch {
    Write-Host "   ❌ Could not check server status" -ForegroundColor Red
    $allGood = $false
}

# Check 2: GeyserMC
Write-Host ""
Write-Host "2. Checking GeyserMC..." -ForegroundColor Yellow
try {
    $geyserProcess = Get-Process -Name "java" -ErrorAction SilentlyContinue | Where-Object {
        $_.Path -like "*geyser*" -or (Get-WmiObject Win32_Process -Filter "ProcessId = $($_.Id)" | Select-Object -ExpandProperty CommandLine) -like "*geyser.jar*"
    }
    
    # Check if port is listening (UDP is harder to test, so check process)
    $javaProcesses = Get-Process -Name "java" -ErrorAction SilentlyContinue
    $geyserRunning = $false
    
    foreach ($proc in $javaProcesses) {
        $cmdLine = (Get-CimInstance Win32_Process -Filter "ProcessId = $($proc.Id)").CommandLine
        if ($cmdLine -like "*geyser.jar*") {
            $geyserRunning = $true
            break
        }
    }
    
    if ($geyserRunning) {
        Write-Host "   ✅ GeyserMC is RUNNING" -ForegroundColor Green
    } else {
        Write-Host "   ⚠️  GeyserMC may not be running" -ForegroundColor Yellow
        Write-Host "      → Start with: .\start-geyser.ps1" -ForegroundColor Yellow
        Write-Host "      → (If Bedrock players don't need to join, this is OK)" -ForegroundColor Gray
    }
} catch {
    Write-Host "   ⚠️  Could not check GeyserMC status" -ForegroundColor Yellow
}

# Check 3: Server Properties
Write-Host ""
Write-Host "3. Checking Server Configuration..." -ForegroundColor Yellow
$serverProps = "server\server.properties"
if (Test-Path $serverProps) {
    $props = Get-Content $serverProps -Raw
    
    if ($props -match "view-distance=(\d+)") {
        $viewDist = $matches[1]
        Write-Host "   ✅ View Distance: $viewDist chunks" -ForegroundColor Green
    }
    
    if ($props -match "simulation-distance=(\d+)") {
        $simDist = $matches[1]
        Write-Host "   ✅ Simulation Distance: $simDist chunks" -ForegroundColor Green
    }
    
    if ($props -match "max-players=(\d+)") {
        $maxPlayers = $matches[1]
        Write-Host "   ✅ Max Players: $maxPlayers" -ForegroundColor Green
    }
} else {
    Write-Host "   ⚠️  Server properties file not found" -ForegroundColor Yellow
}

# Check 4: World Status
Write-Host ""
Write-Host "4. Checking World Status..." -ForegroundColor Yellow
$worldDir = "server\world"
if (Test-Path $worldDir) {
    $worldSize = (Get-ChildItem $worldDir -Recurse -File -ErrorAction SilentlyContinue | 
        Measure-Object -Property Length -Sum).Sum / 1GB
    $worldSizeMB = [math]::Round($worldSize * 1024, 2)
    
    Write-Host "   ✅ World exists: $worldSizeMB MB" -ForegroundColor Green
    
    # Check for recent backups
    $backupDir = "backups"
    if (Test-Path $backupDir) {
        $backups = Get-ChildItem $backupDir -Directory | Sort-Object LastWriteTime -Descending
        if ($backups.Count -gt 0) {
            $latestBackup = $backups[0]
            $backupAge = (Get-Date) - $latestBackup.LastWriteTime
            if ($backupAge.TotalHours -lt 24) {
                Write-Host "   ✅ Latest backup: $($latestBackup.Name) ($([math]::Round($backupAge.TotalHours, 1)) hours ago)" -ForegroundColor Green
            } else {
                Write-Host "   ⚠️  Latest backup: $($latestBackup.Name) ($([math]::Round($backupAge.TotalDays, 1)) days ago)" -ForegroundColor Yellow
                Write-Host "      → Consider setting up auto-backups: .\setup-auto-backup.ps1" -ForegroundColor Cyan
            }
        } else {
            Write-Host "   ⚠️  No backups found" -ForegroundColor Yellow
            Write-Host "      → Setup auto-backups: .\setup-auto-backup.ps1" -ForegroundColor Cyan
        }
    } else {
        Write-Host "   ⚠️  No backups folder found" -ForegroundColor Yellow
    }
} else {
    Write-Host "   ⚠️  World directory not found (server may not have started yet)" -ForegroundColor Yellow
}

# Check 5: playit.gg Reminder
Write-Host ""
Write-Host "5. playit.gg Status..." -ForegroundColor Yellow
Write-Host "   ℹ️  Remember to keep playit.gg application running" -ForegroundColor Cyan
Write-Host "      → Check dashboard: https://playit.gg/account/agents" -ForegroundColor Cyan
Write-Host "      → Both Java and Bedrock tunnels should be active" -ForegroundColor Cyan

# Summary
Write-Host ""
Write-Host "=== Summary ===" -ForegroundColor Cyan
if ($allGood) {
    Write-Host "✅ Server is running!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "  • Java players: Use your playit.gg Java tunnel address" -ForegroundColor White
    Write-Host "  • Bedrock players: Use your playit.gg Bedrock tunnel address" -ForegroundColor White
    Write-Host "  • Check playit.gg dashboard for current addresses" -ForegroundColor White
} else {
    Write-Host "⚠️  Some issues found - check above" -ForegroundColor Yellow
}

Write-Host ""



