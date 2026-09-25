# Application Icon Setup

## Current Status
I've created a tools-themed SVG icon (`icon.svg`) with:
- 🔧 Wrench
- 🔩 Screwdriver  
- ⚙️ Gear
- Blue modern design

## To Add the Icon to Your Application:

### Option 1: Use an Online Converter (Easiest)
1. Go to https://convertio.co/svg-ico/ or https://cloudconvert.com/svg-to-ico
2. Upload `icon.svg`
3. Convert to ICO format with multiple sizes (16x16, 32x32, 48x48, 256x256)
4. Download as `app.ico`
5. Place `app.ico` in the project root folder
6. Rebuild with `build-app.bat`

### Option 2: Use GIMP (Free Software)
1. Install GIMP (https://www.gimp.org/)
2. Open `icon.svg` in GIMP
3. Export as → `app.ico`
4. Select multiple sizes: 16x16, 32x32, 48x48, 256x256
5. Save to project root
6. Rebuild with `build-app.bat`

### Option 3: Use PowerShell + .NET (No extra software)
Run this PowerShell script in the project directory:

```powershell
# This will create a basic icon from the SVG
# Note: For best results, use Option 1 or 2 above
Add-Type -AssemblyName System.Drawing
$svg = [System.IO.File]::ReadAllText("icon.svg")
# Manual conversion needed - use online tool recommended
```

## The Icon Will Show:
- ✅ In the window title bar
- ✅ In the taskbar when running
- ✅ In Windows Explorer as the file icon
- ✅ In the Start menu (if pinned)

## Already Configured
The project file (`HardwareDetectorApp.csproj`) is already set to use `app.ico` as the application icon. Just create the .ico file and rebuild!
