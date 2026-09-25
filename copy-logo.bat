@echo off
echo ================================================
echo DetectIt - Icon Setup Helper
echo ================================================
echo.
echo This script helps you set up the application icon.
echo.

if exist "logo.png" (
    echo [OK] logo.png found in project folder!
    echo.
    echo Next steps:
    echo 1. Go to https://convertio.co/png-ico/
    echo 2. Upload logo.png
    echo 3. Download the converted .ico file
    echo 4. Save as app.ico in this folder
    echo 5. Run create-installer.bat
    echo.
) else if exist "icon.svg" (
    echo [INFO] icon.svg found (SVG format)
    echo.
    echo Next steps:
    echo 1. Go to https://convertio.co/svg-ico/
    echo 2. Upload icon.svg
    echo 3. Download the converted .ico file
    echo 4. Save as app.ico in this folder
    echo 5. Run create-installer.bat
    echo.
) else (
    echo [WARNING] No logo file found!
    echo.
    echo Please place your logo file in this folder as:
    echo - logo.png OR
    echo - icon.svg
    echo.
    echo Then run this script again.
    echo.
)

if exist "app.ico" (
    echo ===================================
    echo SUCCESS! Icon Ready!
    echo ===================================
    echo.
    echo app.ico exists and is ready to use!
    echo.
    echo You can now run:
    echo - quick-test.bat (to test build)
    echo - create-installer.bat (to create installer)
    echo.
    dir app.ico | find "app.ico"
    echo.
) else (
    echo ===================================
    echo Icon Not Yet Created
    echo ===================================
    echo.
    echo Follow the steps above to create app.ico
    echo.
)

pause
