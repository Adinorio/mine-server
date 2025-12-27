# Troubleshoot: One Player Can't Join (Others Can)
Write-Host "=== Troubleshooting: One Friend Can't Join ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "Since other friends CAN connect, the server is working correctly." -ForegroundColor Green
Write-Host "The issue is likely specific to this friend's connection or setup." -ForegroundColor Yellow
Write-Host ""

Write-Host "=== Step 1: Check Which Connection Method Works ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "Ask your friend which address they're trying to use:" -ForegroundColor Yellow
Write-Host ""
Write-Host "Option A: External Address (for friends NOT on same WiFi)" -ForegroundColor White
Write-Host "  run-generous.gl.joinmc.link:37795" -ForegroundColor Green
Write-Host "  OR" -ForegroundColor Gray
Write-Host "  147.185.221.24:37795" -ForegroundColor Green
Write-Host ""
Write-Host "Option B: Local WiFi Address (for friends on SAME WiFi)" -ForegroundColor White
Write-Host "  192.168.1.4:25565" -ForegroundColor Green
Write-Host ""

Write-Host "=== Step 2: Determine Friend's Location ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "Ask your friend:" -ForegroundColor Yellow
Write-Host "  1. Are they on the SAME WiFi as you?" -ForegroundColor White
Write-Host "     → If YES: Use local address: 192.168.1.4:25565" -ForegroundColor Green
Write-Host "     → If NO: Use external address (playit.gg)" -ForegroundColor Green
Write-Host ""
Write-Host "  2. Are they on a DIFFERENT WiFi or mobile data?" -ForegroundColor White
Write-Host "     → Use: run-generous.gl.joinmc.link:37795" -ForegroundColor Green
Write-Host "     → OR try IP: 147.185.221.24:37795" -ForegroundColor Green
Write-Host ""

Write-Host "=== Step 3: Try Different Addresses ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "Have your friend try these addresses IN ORDER:" -ForegroundColor Yellow
Write-Host ""
Write-Host "For External Connection (different WiFi/Internet):" -ForegroundColor White
Write-Host "  1. Try IP address first: 147.185.221.24:37795" -ForegroundColor Green
Write-Host "  2. Try domain: run-generous.gl.joinmc.link:37795" -ForegroundColor Green
Write-Host ""
Write-Host "For Local WiFi (same network):" -ForegroundColor White
Write-Host "  1. Try: 192.168.1.4:25565" -ForegroundColor Green
Write-Host ""

Write-Host "=== Step 4: Common Issues & Fixes ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "Issue 1: 'Cannot connect to server'" -ForegroundColor Yellow
Write-Host "  → Wrong address or network issue" -ForegroundColor Gray
Write-Host "  → Try the OTHER address (local vs external)" -ForegroundColor Gray
Write-Host ""
Write-Host "Issue 2: 'Connection timed out'" -ForegroundColor Yellow
Write-Host "  → Friend's firewall/router blocking connection" -ForegroundColor Gray
Write-Host "  → Try using IP address instead of domain" -ForegroundColor Gray
Write-Host ""
Write-Host "Issue 3: 'Unknown host' or 'Cannot resolve hostname'" -ForegroundColor Yellow
Write-Host "  → DNS issue on friend's computer" -ForegroundColor Gray
Write-Host "  → Use IP address: 147.185.221.24:37795" -ForegroundColor Gray
Write-Host ""
Write-Host "Issue 4: 'Connection refused'" -ForegroundColor Yellow
Write-Host "  → Server might be full or friend is banned" -ForegroundColor Gray
Write-Host "  → Check server console for errors" -ForegroundColor Gray
Write-Host ""

Write-Host "=== Step 5: Check Server Console ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "When friend tries to connect, check your server window:" -ForegroundColor Yellow
Write-Host "  → Does it show connection attempt?" -ForegroundColor White
Write-Host "  → Any error messages?" -ForegroundColor White
Write-Host "  → If NO connection attempt shown: Wrong address or tunnel issue" -ForegroundColor Gray
Write-Host "  → If connection attempt shown: Server-side issue" -ForegroundColor Gray
Write-Host ""

Write-Host "=== Step 6: Verify Current Connection Info ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "Local WiFi Address (same WiFi only):" -ForegroundColor White
Write-Host "  192.168.1.4:25565" -ForegroundColor Green
Write-Host ""
Write-Host "External Address (different WiFi/Internet):" -ForegroundColor White
Write-Host "  run-generous.gl.joinmc.link:37795" -ForegroundColor Green
Write-Host "  OR IP: 147.185.221.24:37795" -ForegroundColor Green
Write-Host ""
Write-Host "Note: External address may change - check playit.gg dashboard!" -ForegroundColor Yellow
Write-Host ""

Write-Host "=== Quick Checklist ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "□ Server is running (you confirmed this)" -ForegroundColor White
Write-Host "□ playit.gg is running (if using external address)" -ForegroundColor White
Write-Host "□ Friend is using correct address for their location" -ForegroundColor White
Write-Host "□ Friend tried BOTH IP and domain address" -ForegroundColor White
Write-Host "□ Friend checked their firewall/antivirus" -ForegroundColor White
Write-Host "□ Server console shows connection attempt (if external)" -ForegroundColor White
Write-Host ""

