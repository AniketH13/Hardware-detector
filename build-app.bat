@echo off
echo ============================================
echo DetectIt - Windows Application
echo ============================================
echo.
echo Building Windows GUI Application...
echo.

REM Clean previous builds
if exist "bin\Release" rmdir /s /q "bin\Release"
if exist "publish-app" rmdir /s /q "publish-app"

REM Build the Windows Forms application
dotnet publish DetectIt.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish-app

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ===================================
    echo Build Successful!
    echo ===================================
    echo.
    echo Application created at: .\publish-app\DetectIt.exe
    echo.
    echo File size:
    dir publish-app\DetectIt.exe | find "DetectIt.exe"
    echo.
    echo This is a standalone Windows application with a graphical interface.
    echo No installation required - just run the .exe file!
    echo.
    echo Features:
    echo - Modern graphical user interface
    echo - Hardware information displayed in organized categories
    echo - Export to text file
    echo - No console window
    echo.
) else (
    echo.
    echo ===================================
    echo Build Failed! Check errors above.
    echo ===================================
)

pause
