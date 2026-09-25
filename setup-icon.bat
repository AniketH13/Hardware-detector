@echo off
echo ===============================================
echo Hardware Detector Icon Setup
echo ===============================================
echo.
echo This script will help you add a custom icon to your application.
echo.
echo STEPS:
echo 1. Convert icon.svg to app.ico using one of these methods:
echo    - Online: https://convertio.co/svg-ico/
echo    - GIMP: File ^> Export As ^> app.ico
echo.
echo 2. Place app.ico in this folder
echo.
echo 3. Run build-app.bat to rebuild with the new icon
echo.
echo Current status:

if exist "app.ico" (
    echo [OK] app.ico found!
    dir app.ico | find "app.ico"
    echo.
    echo Ready to build! Run build-app.bat
) else (
    echo [!] app.ico NOT found
    echo.
    echo Please create app.ico from icon.svg first.
    echo See ICON-SETUP.md for instructions.
)

echo.
pause
