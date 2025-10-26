#!/bin/bash

# ============================================
# Run Mini E-Commerce Application
# Linux/macOS Bash Script
# ============================================

echo "========================================"
echo "  Mini E-Commerce - Strategy Pattern"
echo "========================================"
echo ""

# Check if .NET Runtime is installed
echo "Checking .NET Runtime..."
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET Runtime not found!"
    echo "Please install .NET 8.0 Runtime from:"
    echo "https://dotnet.microsoft.com/download/dotnet/8.0"
    echo ""
    read -p "Press Enter to exit"
    exit 1
fi

dotnetVersion=$(dotnet --version)
echo "Found .NET version: $dotnetVersion"
echo ""

# Navigate to application directory
cd "$(dirname "$0")/MiniECommerce" || exit

# Check if DLL exists
if [ ! -f "MiniECommerce.API.dll" ]; then
    echo "ERROR: MiniECommerce.API.dll not found!"
    echo "Please ensure the application files are in the correct location."
    echo ""
    read -p "Press Enter to exit"
    exit 1
fi

# Display startup info
echo "Starting Mini E-Commerce API..."
echo ""
echo "Application will be available at:"
echo "  HTTP: http://localhost:5000"
echo "  Swagger UI: http://localhost:5000/swagger"
echo ""
echo "Default accounts:"
echo "  Admin: admin@ecommerce.com / admin123"
echo "  User:  user@ecommerce.com / user123"
echo ""
echo "Press Ctrl+C to stop the application"
echo "========================================"
echo ""

# Run the application
dotnet MiniECommerce.API.dll
