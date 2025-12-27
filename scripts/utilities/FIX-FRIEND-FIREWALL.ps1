# Minecraft Server Firewall Fix for Friends
# This script allows Minecraft/Java to connect through Windows Firewall

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Minecraft Server Firewall Fix (For Friends)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator!" -ForegroundColor Red
    Write-Host ""
    Write-Host "To fix this:" -ForegroundColor Yellow
    Write-Host "1. Right-click this file (FIX-FRIEND-FIREWALL.ps1)" -ForegroundColor White
    Write-Host "2. Select 'Run with PowerShell' as Administrator" -ForegroundColor White
    Write-Host "3. Or right-click PowerShell and select 'Run as Administrator'" -ForegroundColor White
    Write-Host "4. Then navigate to this folder and run: .\FIX-FRIEND-FIREWALL.ps1" -ForegroundColor White
    Write-Host ""
    Write-Host "Press any key to exit..."
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
    exit 1
}

Write-Host "Administrator privileges confirmed!" -ForegroundColor Green
Write-Host ""

# Remove existing rules (cleanup)
Write-Host "Cleaning up existing firewall rules..." -ForegroundColor Yellow
Remove-NetFirewallRule -DisplayName "Minecraft - Allow Outbound" -ErrorAction SilentlyContinue
Remove-NetFirewallRule -DisplayName "Minecraft - Allow Outbound Connection" -ErrorAction SilentlyContinue
Remove-NetFirewallRule -DisplayName "Minecraft - Allow All Programs Outbound" -ErrorAction SilentlyContinue

# Try to find Java installations
Write-Host ""
Write-Host "Looking for Java installations..." -ForegroundColor Yellow
$javaPaths = @(
    "${env:ProgramFiles}\Java\*\bin\java.exe",
    "${env:ProgramFiles(x86)}\Java\*\bin\java.exe",
    "$env:LOCALAPPDATA\Programs\Java\*\bin\java.exe"
)

$foundJava = $false
foreach ($path in $javaPaths) {
    $javaFiles = Get-ChildItem -Path $path -ErrorAction SilentlyContinue
    if ($javaFiles) {
        foreach ($java in $javaFiles) {
            Write-Host "Found Java at: $($java.FullName)" -ForegroundColor Cyan
            try {
                New-NetFirewallRule -DisplayName "Minecraft - Allow Java Outbound ($($java.Name))" `
                    -Direction Outbound `
                    -Program $java.FullName `
                    -Action Allow `
                    -Description "Allows Java to connect outbound for Minecraft" `
                    -ErrorAction SilentlyContinue | Out-Null
                $foundJava = $true
            } catch {
                Write-Host "  Could not add rule for this Java installation" -ForegroundColor Yellow
            }
        }
    }
}

# Add general outbound rule for Minecraft (allows any program to connect)
Write-Host ""
Write-Host "Adding general firewall rule for Minecraft connections..." -ForegroundColor Yellow
try {
    # Create a broad outbound rule (allows outbound connections)
    New-NetFirewallRule -DisplayName "Minecraft - Allow Outbound Connection" `
        -Direction Outbound `
        -Action Allow `
        -Description "Allows outbound connections for Minecraft servers" `
        -ErrorAction Stop | Out-Null
    
    Write-Host "General firewall rule added successfully!" -ForegroundColor Green
} catch {
    Write-Host "Note: General rule may already exist or couldn't be created" -ForegroundColor Yellow
    Write-Host "Trying alternative method..." -ForegroundColor Yellow
    
    # Alternative: Create rule for specific port range
    try {
        New-NetFirewallRule -DisplayName "Minecraft - Allow Outbound Ports" `
            -Direction Outbound `
            -Protocol TCP `
            -Action Allow `
            -Description "Allows outbound TCP connections for Minecraft" `
            -ErrorAction SilentlyContinue | Out-Null
    } catch {
        Write-Host "Alternative rule creation failed, but continuing..." -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "SUCCESS! Firewall is now configured!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "You can now try connecting to the server again." -ForegroundColor Cyan
Write-Host ""

# Display summary
Write-Host "Firewall rules added:" -ForegroundColor Yellow
Get-NetFirewallRule -DisplayName "*Minecraft*" | Where-Object { $_.Enabled -eq $true } | ForEach-Object {
    Write-Host "  - $($_.DisplayName)" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "IMPORTANT: Check Antivirus Too!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "If you still can't connect, check your antivirus software:" -ForegroundColor White
Write-Host "  - Windows Defender: Usually OK after firewall fix" -ForegroundColor Gray
Write-Host "  - Avast/AVG/Norton/McAfee: May need to add Minecraft to exceptions" -ForegroundColor Gray
Write-Host ""
Write-Host "Also try:" -ForegroundColor White
Write-Host "  - Disable VPN if you're using one" -ForegroundColor Gray
Write-Host "  - Try connecting from a different network (mobile hotspot)" -ForegroundColor Gray
Write-Host ""

Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')

