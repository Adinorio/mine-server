# Setup ngrok for Minecraft Server
Write-Host "=== ngrok Setup for Minecraft Server ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "ngrok is a more reliable alternative to playit.gg for Minecraft" -ForegroundColor Yellow
Write-Host ""

Write-Host "Steps to set up ngrok:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Sign up for free at: https://ngrok.com/" -ForegroundColor White
Write-Host "2. Download ngrok for Windows" -ForegroundColor White
Write-Host "3. Extract ngrok.exe to a folder (e.g., C:\ngrok\)" -ForegroundColor White
Write-Host "4. Get your authtoken from: https://dashboard.ngrok.com/get-started/your-authtoken" -ForegroundColor White
Write-Host "5. Run: ngrok config add-authtoken YOUR_TOKEN" -ForegroundColor White
Write-Host "6. Start tunnel: ngrok tcp 25565" -ForegroundColor White
Write-Host "7. Use the address shown (e.g., 0.tcp.ngrok.io:12345)" -ForegroundColor White
Write-Host ""

Write-Host "Alternative: Use local network (works now!)" -ForegroundColor Green
Write-Host "  Address: 192.168.1.8:25565" -ForegroundColor White
Write-Host "  Works for anyone on the same WiFi" -ForegroundColor White











