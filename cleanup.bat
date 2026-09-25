@echo off
echo ================================================
echo DetectIt - Project Cleanup
echo ================================================
echo.
echo Cleaning up build artifacts and temporary files...
echo.

REM Remove build directories
if exist "bin" (
    echo Removing bin\...
    rmdir /s /q "bin"
)

if exist "obj" (
    echo Removing obj\...
    rmdir /s /q "obj"
)

REM Remove publish directories
if exist "publish" (
    echo Removing publish\...
    rmdir /s /q "publish"
)

if exist "publish-app" (
    echo Removing publish-app\...
    rmdir /s /q "publish-app"
)

if exist "publish-msi" (
    echo Removing publish-msi\...
    rmdir /s /q "publish-msi"
)

if exist "publish-installer" (
    echo Removing publish-installer\...
    rmdir /s /q "publish-installer"
)

REM Remove WiX temporary files
if exist "*.wixobj" (
    echo Removing WiX object files...
    del /q *.wixobj
)

if exist "*.wixpdb" (
    echo Removing WiX PDB files...
    del /q *.wixpdb
)

echo.
echo ===================================
echo Cleanup Complete!
echo ===================================
echo.
echo Removed:
echo - Build artifacts (bin, obj)
echo - Publish directories
echo - Temporary files
echo.
echo Project is now clean and ready for fresh build.
echo.

pause
