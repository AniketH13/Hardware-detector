# Hardware Detector - Complete Installation Guide

## 📋 Prerequisites

Before you begin, make sure you have:

1. **Windows 10 or 11** (64-bit)
2. **.NET 8.0 SDK** installed
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
3. **Inno Setup** (for creating installer)
   - Download: https://jrsoftware.org/isdl.php

---

## 🚀 Quick Start (5 Minutes)

### Step 1: Install .NET 8.0 SDK
1. Go to https://dotnet.microsoft.com/download/dotnet/8.0
2. Download "SDK x64" installer
3. Run installer and follow prompts
4. Restart Command Prompt/PowerShell

### Step 2: Create Application Icon
1. Go to https://convertio.co/svg-ico/
2. Upload `icon.svg` from project folder
3. Download the converted file
4. Save as `app.ico` in project root folder

### Step 3: Install Inno Setup
1. Download from https://jrsoftware.org/isdl.php
2. Run installer (use default options)
3. Complete installation

### Step 4: Build the Installer
1. Open Command Prompt or PowerShell
2. Navigate to project folder:
   ```bash
   cd C:\Users\Aniket\HardwareDetector
   ```
3. Run the installer builder:
   ```bash
   create-installer.bat
   ```
4. Wait for build to complete (2-3 minutes)

### Step 5: Get Your Installer
Your installer will be at:
```
C:\Users\Aniket\HardwareDetector\Output\HardwareDetector_Setup_v1.0.0.exe
```

---

## 📦 What Gets Built

### Installer Package Includes:
- ✅ Hardware Detector application
- ✅ All required .NET libraries
- ✅ Installation wizard
- ✅ Start Menu shortcuts
- ✅ Desktop shortcut (optional)
- ✅ Uninstaller

### File Size:
- **Installer**: ~75-85 MB
- **Installed Size**: ~85-95 MB

---

## 🔧 Installation Process (For End Users)

### How Users Install Your App:

1. **Double-click** `HardwareDetector_Setup_v1.0.0.exe`

2. **Welcome Screen** appears
   - Click "Next"

3. **License Agreement**
   - Read and accept
   - Click "Next"

4. **Choose Installation Folder**
   - Default: `C:\Program Files\Hardware Detector`
   - Or choose custom location
   - Click "Next"

5. **Select Start Menu Folder**
   - Default: "Hardware Detector"
   - Click "Next"

6. **Additional Tasks**
   - ☑ Create desktop shortcut (optional)
   - Click "Next"

7. **Ready to Install**
   - Review settings
   - Click "Install"

8. **Installing**
   - Progress bar shows installation
   - Takes 10-30 seconds

9. **Completed**
   - ☑ Launch Hardware Detector (optional)
   - Click "Finish"

---

## 🗑️ How Users Uninstall

### Method 1: Windows Settings
1. Open **Settings** → **Apps** → **Installed apps**
2. Find "Hardware Detector"
3. Click **⋮** (three dots) → **Uninstall**
4. Confirm uninstallation

### Method 2: Control Panel
1. Open **Control Panel** → **Programs** → **Programs and Features**
2. Find "Hardware Detector"
3. Right-click → **Uninstall**
4. Follow uninstall wizard

### Method 3: Start Menu
1. Open **Start Menu**
2. Find "Hardware Detector" folder
3. Click **Uninstall Hardware Detector**
4. Follow uninstall wizard

---

## 🧪 Testing Your Build

### Before Distributing, Test:

1. **Build Test**
   ```bash
   quick-test.bat
   ```
   ✅ Should show "BUILD SUCCESSFUL"

2. **Run Application**
   ```bash
   dotnet run --project HardwareDetectorApp.csproj
   ```
   ✅ Application should launch
   ✅ Hardware info should display

3. **Create Installer**
   ```bash
   create-installer.bat
   ```
   ✅ Should create `Output\HardwareDetector_Setup_v1.0.0.exe`

4. **Test Installation**
   - Run the installer
   - Complete installation
   - Launch from Start Menu
   - Test all features
   - Uninstall cleanly

---

## 📤 Distribution

### Share Your Installer:

**Option 1: Direct Download**
- Upload `HardwareDetector_Setup_v1.0.0.exe` to cloud storage
- Share download link

**Option 2: GitHub Release**
- Create release on GitHub
- Attach installer as release asset

**Option 3: Website**
- Host on your website
- Provide download button

**Option 4: USB Drive**
- Copy installer to USB
- Users can install offline

---

## ⚙️ Customization Options

### Change Company Name:
Edit these files:
- `HardwareDetector.csproj` (line 12)
- `installer-script.iss` (line 4)

### Change Version Number:
Edit:
- `HardwareDetector.csproj` (line 14)
- `installer-script.iss` (line 3)

### Change Application Name:
Edit:
- `installer-script.iss` (line 2)

Then rebuild with `create-installer.bat`

---

## 🐛 Troubleshooting

### "dotnet command not found"
**Fix**: Install .NET 8.0 SDK and restart terminal

### "ISCC.exe not found"
**Fix**: Install Inno Setup from https://jrsoftware.org/isdl.php

### "app.ico not found"
**Fix**: Convert `icon.svg` to `app.ico` using online converter

### Build fails with error
**Fix**: 
1. Run `quick-test.bat` to see specific errors
2. Check all prerequisites are installed
3. Ensure no files are open in other programs

### Installer doesn't include icon
**Fix**: Make sure `app.ico` exists before building

---

## 📞 Support

For issues or questions:
- Check `QA-REPORT.md` for known issues
- Check `FIXES-APPLIED.md` for recent changes
- Review build output for error messages

---

## ✅ Final Checklist

Before distributing your installer:

- [ ] .NET 8.0 SDK installed
- [ ] Inno Setup installed
- [ ] `app.ico` created from `icon.svg`
- [ ] `quick-test.bat` runs successfully
- [ ] `create-installer.bat` completes without errors
- [ ] Installer file created in `Output\` folder
- [ ] Tested installation on clean Windows system
- [ ] Application launches and shows hardware
- [ ] Export function works
- [ ] Uninstaller removes application completely
- [ ] No leftover files after uninstall

---

**Estimated Total Setup Time**: 15-20 minutes
**Build Time**: 2-3 minutes per build

Good luck with your Hardware Detector application! 🚀
