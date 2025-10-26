# ============================================
# Run Mini E-Commerce Application
# Windows PowerShell Script
# ============================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Mini E-Commerce - Strategy Pattern" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET Runtime is installed
Write-Host "Checking .NET Runtime..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: .NET Runtime not found!" -ForegroundColor Red
    Write-Host "Please install .NET 8.0 Runtime from:" -ForegroundColor Red
    Write-Host "https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}
Write-Host "Found .NET version: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Navigate to application directory
Set-Location -Path "$PSScriptRoot\MiniECommerce"

# Check if DLL exists
if (-not (Test-Path "MiniECommerce.API.dll")) {
    Write-Host "ERROR: MiniECommerce.API.dll not found!" -ForegroundColor Red
    Write-Host "Please ensure the application files are in the correct location." -ForegroundColor Red
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

# Display startup info
Write-Host "Starting Mini E-Commerce API..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Application will be available at:" -ForegroundColor Green
Write-Host "  HTTP: http://localhost:5000" -ForegroundColor Cyan
Write-Host "  Swagger UI: http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "Default accounts:" -ForegroundColor Green
Write-Host "  Admin: admin@ecommerce.com / admin123" -ForegroundColor Cyan
Write-Host "  User:  user@ecommerce.com / user123" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Ctrl+C to stop the application" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Run the application
dotnet MiniECommerce.API.dll
