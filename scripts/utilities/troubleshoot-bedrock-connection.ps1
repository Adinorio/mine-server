# Troubleshoot Bedrock Connection Issues

Write-Host "=== Troubleshooting Bedrock Connection ===" -ForegroundColor Cyan
Write-Host ""

$issuesFound = 0

# Check 1: Is Java server running?
Write-Host "1. Checking if Java server is running..." -ForegroundColor Yellow
try {
    $serverConnection = Test-NetConnection -ComputerName "127.0.0.1" -Port 25565 -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
    if ($serverConnection.TcpTestSucceeded) {
        Write-Host "   ✅ Java server is running on port 25565" -ForegroundColor Green
    } else {
        Write-Host "   ❌ Java server is NOT running on port 25565" -ForegroundColor Red
        Write-Host "      → Start server: .\start-server.ps1" -ForegroundColor Yellow
        $issuesFound++
    }
} catch {
    Write-Host "   ❌ Could not check server status" -ForegroundColor Red
    $issuesFound++
}

# Check 2: Is GeyserMC running?
Write-Host ""
Write-Host "2. Checking if GeyserMC is running..." -ForegroundColor Yellow
try {
    $geyserConnection = Test-NetConnection -ComputerName "127.0.0.1" -Port 19132 -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
    if ($geyserConnection.TcpTestSucceeded -or $geyserConnection.UdpTestSucceeded) {
        Write-Host "   ✅ GeyserMC appears to be running on port 19132" -ForegroundColor Green
    } else {
        Write-Host "   ❌ GeyserMC is NOT running on port 19132" -ForegroundColor Red
        Write-Host "      → Start Geyser: .\start-geyser.ps1" -ForegroundColor Yellow
        $issuesFound++
    }
} catch {
    Write-Host "   ⚠️  Could not check GeyserMC (UDP ports are hard to test)" -ForegroundColor Yellow
    Write-Host "      → Make sure Geyser is running: .\start-geyser.ps1" -ForegroundColor Yellow
}

# Check 3: Check Geyser config
Write-Host ""
Write-Host "3. Checking Geyser configuration..." -ForegroundColor Yellow
$geyserConfig = "geyser\config.yml"
if (Test-Path $geyserConfig) {
    $configContent = Get-Content $geyserConfig -Raw
    
    if ($configContent -match "address:\s*127\.0\.0\.1" -or $configContent -match "address:\s*'127\.0\.0\.1'") {
        Write-Host "   ✅ Geyser config: Java server address is 127.0.0.1" -ForegroundColor Green
    } else {
        Write-Host "   ⚠️  Geyser config: Check that java.address is 127.0.0.1" -ForegroundColor Yellow
    }
    
    if ($configContent -match "port:\s*25565") {
        Write-Host "   ✅ Geyser config: Java server port is 25565" -ForegroundColor Green
    } else {
        Write-Host "   ⚠️  Geyser config: Check that java.port is 25565" -ForegroundColor Yellow
    }
    
    if ($configContent -match "port:\s*19132") {
        Write-Host "   ✅ Geyser config: Bedrock port is 19132" -ForegroundColor Green
    } else {
        Write-Host "   ⚠️  Geyser config: Check that bedrock.port is 19132" -ForegroundColor Yellow
    }
    
    if ($configContent -match "auth-type:\s*offline") {
        Write-Host "   ✅ Geyser config: Auth type is offline" -ForegroundColor Green
    } else {
        Write-Host "   ⚠️  Geyser config: Should be auth-type: offline" -ForegroundColor Yellow
    }
} else {
    Write-Host "   ❌ Geyser config file not found!" -ForegroundColor Red
    Write-Host "      → Run setup: .\setup-geyser.ps1" -ForegroundColor Yellow
    $issuesFound++
}

# Check 4: Firewall
Write-Host ""
Write-Host "4. Checking firewall..." -ForegroundColor Yellow
Write-Host "   → Make sure port 19132 (UDP) is allowed" -ForegroundColor Cyan
Write-Host "   → Run: .\FIX-FIREWALL-BEDROCK.bat" -ForegroundColor Cyan

# Summary
Write-Host ""
Write-Host "=== Summary ===" -ForegroundColor Cyan
if ($issuesFound -eq 0) {
    Write-Host "✅ All checks passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "If connection still fails:" -ForegroundColor Yellow
    Write-Host "1. Check playit.gg Bedrock tunnel is active" -ForegroundColor White
    Write-Host "2. Verify connection address: 147.185.221.16:23534" -ForegroundColor White
    Write-Host "3. Check playit.gg dashboard for correct port" -ForegroundColor White
} else {
    Write-Host "⚠️  Found $issuesFound issue(s) - fix them above" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Connection Info:" -ForegroundColor Cyan
Write-Host "  Address: 147.185.221.16:23534" -ForegroundColor White
Write-Host "  (or: believe-interaction.gl.at.ply.gg:23534)" -ForegroundColor Gray
Write-Host ""



