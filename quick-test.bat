@echo off
echo ================================================
echo DetectIt - Quick Build Test
echo ================================================
echo.
echo This script tests if the project builds successfully
echo without creating installer packages.
echo.

echo Testing build...
dotnet build HardwareDetectorApp.csproj -c Release

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ===================================
    echo BUILD SUCCESSFUL!
    echo ===================================
    echo.
    echo No critical errors found.
    echo.
    echo Next steps:
    echo 1. Convert icon.svg to app.ico
    echo 2. Run create-installer.bat to create installer
    echo.
) else (
    echo.
    echo ===================================
    echo BUILD FAILED!
    echo ===================================
    echo.
    echo Check error messages above.
    echo.
)

pause
