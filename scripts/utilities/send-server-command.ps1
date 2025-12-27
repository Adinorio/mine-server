# Send Command to Minecraft Server
# Usage: .\send-server-command.ps1 "kill mrjoe20"

param(
    [Parameter(Mandatory=$true)]
    [string]$Command
)

Write-Host "=== Sending Command to Server ===" -ForegroundColor Cyan
Write-Host ""

# Check if server is running
$serverRunning = Test-NetConnection -ComputerName 127.0.0.1 -Port 25565 -InformationLevel Quiet -WarningAction SilentlyContinue

if (-not $serverRunning) {
    Write-Host "❌ Server is NOT running!" -ForegroundColor Red
    Write-Host "   Start server with: .\start-server.ps1" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Server is running" -ForegroundColor Green
Write-Host ""
Write-Host "⚠️  Note: This script can only send commands if the server was started" -ForegroundColor Yellow
Write-Host "   through the GUI. If you started it manually, use the server console instead." -ForegroundColor Yellow
Write-Host ""
Write-Host "Command to send: $Command" -ForegroundColor Cyan
Write-Host ""

# Try to find the Java process running the server
$serverProcess = Get-Process -Name "java" -ErrorAction SilentlyContinue | 
    Where-Object { 
        $_.CommandLine -like "*server.jar*" -or 
        $_.Path -like "*java*"
    } | Select-Object -First 1

if ($null -eq $serverProcess) {
    Write-Host "❌ Could not find server process!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternative: Use the server console window directly:" -ForegroundColor Yellow
    Write-Host "  1. Find the server console window (where you started the server)" -ForegroundColor White
    Write-Host "  2. Type: $Command" -ForegroundColor Green
    Write-Host "  3. Press Enter" -ForegroundColor White
    Write-Host ""
    Write-Host "Or if you're in-game, type: /$Command" -ForegroundColor Yellow
    exit 1
}

Write-Host "📝 To execute this command, use one of these methods:" -ForegroundColor Cyan
Write-Host ""
Write-Host "Method 1: Server Console (Recommended)" -ForegroundColor Yellow
Write-Host "  1. Go to the server console window" -ForegroundColor White
Write-Host "  2. Type: $Command" -ForegroundColor Green
Write-Host "  3. Press Enter" -ForegroundColor White
Write-Host ""
Write-Host "Method 2: In-Game (If you're OP)" -ForegroundColor Yellow
Write-Host "  1. Join the server" -ForegroundColor White
Write-Host "  2. Press T to open chat" -ForegroundColor White
Write-Host "  3. Type: /$Command" -ForegroundColor Green
Write-Host "  4. Press Enter" -ForegroundColor White
Write-Host ""

