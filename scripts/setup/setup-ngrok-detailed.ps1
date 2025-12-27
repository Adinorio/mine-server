# Detailed ngrok Setup Guide
Write-Host "=== ngrok Setup for Minecraft Server ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "ngrok is a reliable tunneling service that works well with Minecraft" -ForegroundColor Yellow
Write-Host ""

Write-Host "Step 1: Sign Up" -ForegroundColor Cyan
Write-Host "  - Go to: https://ngrok.com/" -ForegroundColor White
Write-Host "  - Sign up for free account" -ForegroundColor White
Write-Host ""

Write-Host "Step 2: Download ngrok" -ForegroundColor Cyan
Write-Host "  - Download ngrok for Windows" -ForegroundColor White
Write-Host "  - Extract ngrok.exe to a folder (e.g., C:\ngrok\)" -ForegroundColor White
Write-Host ""

Write-Host "Step 3: Get Your Auth Token" -ForegroundColor Cyan
Write-Host "  - Go to: https://dashboard.ngrok.com/get-started/your-authtoken" -ForegroundColor White
Write-Host "  - Copy your authtoken" -ForegroundColor White
Write-Host ""

Write-Host "Step 4: Configure ngrok" -ForegroundColor Cyan
Write-Host "  Open PowerShell and run:" -ForegroundColor White
Write-Host "  cd C:\ngrok" -ForegroundColor Gray
Write-Host "  .\ngrok config add-authtoken YOUR_TOKEN_HERE" -ForegroundColor Gray
Write-Host ""

Write-Host "Step 5: Start Tunnel" -ForegroundColor Cyan
Write-Host "  While your Minecraft server is running, start ngrok:" -ForegroundColor White
Write-Host "  .\ngrok tcp 25565" -ForegroundColor Gray
Write-Host ""
Write-Host "  ngrok will show something like:" -ForegroundColor White
Write-Host "  Forwarding  tcp://0.tcp.ngrok.io:12345 -> localhost:25565" -ForegroundColor Green
Write-Host ""

Write-Host "Step 6: Share Address" -ForegroundColor Cyan
Write-Host "  Friends connect to: 0.tcp.ngrok.io:12345" -ForegroundColor Green
Write-Host "  (Use the actual address ngrok shows you)" -ForegroundColor White
Write-Host ""

Write-Host "Note: Free tier limitations:" -ForegroundColor Yellow
Write-Host "  - Address changes each time you restart ngrok" -ForegroundColor White
Write-Host "  - Limited connections per minute" -ForegroundColor White
Write-Host "  - For permanent address, upgrade to paid plan" -ForegroundColor White
Write-Host ""

Write-Host "To keep ngrok running:" -ForegroundColor Cyan
Write-Host "  - Keep the ngrok window open" -ForegroundColor White
Write-Host "  - Or run in background: Start-Process ngrok -ArgumentList 'tcp','25565'" -ForegroundColor White









