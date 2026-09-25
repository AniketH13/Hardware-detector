@echo off
echo ================================================
echo DetectIt - Windows Installer Package
echo ================================================
echo.
echo This will create a proper Windows installer (.exe)
echo that users can run to install your application.
echo.
pause

REM Check for Inno Setup in multiple locations
set ISCC_PATH=
set ISCC_FOUND=0

REM Check standard locations
if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    set "ISCC_PATH=C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
    set ISCC_FOUND=1
    goto :found
)

if exist "C:\Program Files\Inno Setup 6\ISCC.exe" (
    set "ISCC_PATH=C:\Program Files\Inno Setup 6\ISCC.exe"
    set ISCC_FOUND=1
    goto :found
)

REM Check AppData location (winget install)
if exist "C:\Users\Aniket\AppData\Local\Programs\Antigravity IDE\resources\app\node_modules\innosetup\bin\ISCC.exe" (
    set "ISCC_PATH=C:\Users\Aniket\AppData\Local\Programs\Antigravity IDE\resources\app\node_modules\innosetup\bin\ISCC.exe"
    set ISCC_FOUND=1
    goto :found
)

REM Inno Setup not found
echo.
echo ERROR: Inno Setup not found!
echo.
echo Checked locations:
echo - C:\Program Files (x86)\Inno Setup 6\
echo - C:\Program Files\Inno Setup 6\
echo - C:\Users\Aniket\AppData\Local\Programs\Antigravity IDE\resources\app\node_modules\innosetup\bin\
echo.
echo Please install Inno Setup from:
echo https://jrsoftware.org/isdl.php
echo.
pause
exit /b 1

:found
echo Found Inno Setup!
echo Location: %ISCC_PATH%
echo.

:build
echo Building application...
dotnet publish HardwareDetectorApp.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish-installer

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ===================================
    echo Build Failed!
    echo ===================================
    echo.
    echo Check the error messages above.
    pause
    exit /b 1
)

echo.
echo Creating installer package...
"%ISCC_PATH%" installer-script.iss

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ===================================
    echo SUCCESS! Installer Created!
    echo ===================================
    echo.
    echo Installer location: .\Output\DetectIt_Setup_v1.0.0.exe
    echo.
    dir Output\DetectIt_Setup_v1.0.0.exe | find "DetectIt_Setup"
    echo.
    echo This is a professional Windows installer!
    echo.
    echo Features:
    echo - Professional installation wizard
    echo - Creates Start Menu shortcuts
    echo - Creates Desktop shortcut (optional)
    echo - Adds to Programs and Features
    echo - Proper uninstaller included
    echo.
    echo You can now distribute this installer to users!
    echo.
) else (
    echo.
    echo ===================================
    echo Installer Creation Failed!
    echo ===================================
    echo.
    echo Check the error messages above.
    echo.
)

pause
