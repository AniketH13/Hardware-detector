@echo off
echo ================================================
echo Hardware Detector - MSI Installer (Alternative)
echo ================================================
echo.
echo This creates an MSI installer using WiX Toolset.
echo.
echo Prerequisites:
echo - WiX Toolset v3 or v4 must be installed
echo - Download from: https://wixtoolset.org/releases/
echo - Add WiX to PATH or update this script
echo.
pause

REM Build the application first
echo Building application...
dotnet publish DetectIt.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish-msi

if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    pause
    exit /b 1
)

REM Check for WiX
where candle >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: WiX Toolset not found in PATH!
    echo.
    echo Please install WiX Toolset from:
    echo https://wixtoolset.org/releases/
    echo.
    echo Or use the Inno Setup method instead:
    echo run create-installer.bat
    echo.
    pause
    exit /b 1
)

echo.
echo Creating MSI package...
candle installer.wxs
light -ext WixUIExtension installer.wixobj

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ===================================
    echo SUCCESS! MSI Installer Created!
    echo ===================================
    echo.
    echo Installer: HardwareDetector_Setup.msi
    echo.
) else (
    echo MSI creation failed!
)

pause
