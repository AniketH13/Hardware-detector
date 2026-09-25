@echo off
echo ================================================
echo DetectIt - Inno Setup Verification
echo ================================================
echo.

REM Check for Inno Setup in common locations
set ISCC_FOUND=0
set ISCC_PATH=

if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    set "ISCC_PATH=C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
    set ISCC_FOUND=1
)

if exist "C:\Program Files\Inno Setup 6\ISCC.exe" (
    set "ISCC_PATH=C:\Program Files\Inno Setup 6\ISCC.exe"
    set ISCC_FOUND=1
)

if exist "C:\Program Files (x86)\Inno Setup 5\ISCC.exe" (
    set "ISCC_PATH=C:\Program Files (x86)\Inno Setup 5\ISCC.exe"
    set ISCC_FOUND=1
)

if "%ISCC_FOUND%"=="1" (
    echo ===================================
    echo SUCCESS! Inno Setup Found
    echo ===================================
    echo.
    echo Location: %ISCC_PATH%
    echo.
    echo You're ready to build DetectIt installer!
    echo.
    echo Next steps:
    echo 1. Make sure app.ico exists
    echo 2. Run: create-installer.bat
    echo.
) else (
    echo ===================================
    echo Inno Setup NOT Found
    echo ===================================
    echo.
    echo Inno Setup is not installed on your system.
    echo.
    echo To install:
    echo 1. Go to: https://jrsoftware.org/isdl.php
    echo 2. Download the latest version
    echo 3. Run the installer
    echo 4. Run this script again to verify
    echo.
)

echo.
echo ===================================
echo Other Prerequisites Check
echo ===================================
echo.

REM Check for .NET SDK
dotnet --version >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [OK] .NET SDK is installed
    for /f "tokens=*" %%i in ('dotnet --version') do echo     Version: %%i
) else (
    echo [!] .NET SDK not found
    echo     Download from: https://dotnet.microsoft.com/download/dotnet/8.0
)

echo.

REM Check for app.ico
if exist "app.ico" (
    echo [OK] app.ico exists
    for %%A in ("app.ico") do echo     Size: %%~zA bytes
) else (
    echo [!] app.ico not found
    echo     Convert logo.png using: https://convertio.co/png-ico/
)

echo.

REM Check for logo.png
if exist "logo.png" (
    echo [OK] logo.png exists
    for %%A in ("logo.png") do echo     Size: %%~zA bytes
) else (
    echo [!] logo.png not found
)

echo.
echo ===================================
echo Summary
echo ===================================
echo.

if "%ISCC_FOUND%"=="1" (
    if exist "app.ico" (
        echo You are READY TO BUILD!
        echo.
        echo Run: create-installer.bat
    ) else (
        echo Almost ready! Just need to create app.ico
        echo Convert logo.png to app.ico first.
    )
) else (
    echo Please install Inno Setup first.
    echo See: INNO-SETUP-GUIDE.md
)

echo.
pause
