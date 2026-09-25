# Inno Setup Installation and Configuration Guide

## 📦 Installing Inno Setup

### Step 1: Download Inno Setup

1. **Go to**: https://jrsoftware.org/isdl.php

2. **Download**: Click on **"Inno Setup 6.x.x"** (latest version)
   - File: `innosetup-6.x.x.exe` (~5 MB)

3. **Save** the installer to your Downloads folder

---

### Step 2: Install Inno Setup

1. **Run** the downloaded `innosetup-6.x.x.exe`

2. **Welcome Screen**:
   - Click **"Next"**

3. **License Agreement**:
   - Read and accept
   - Click **"Next"**

4. **Select Destination Location**:
   - Default: `C:\Program Files (x86)\Inno Setup 6`
   - Click **"Next"**

5. **Select Components**:
   - Keep all defaults selected:
     - ☑ Inno Setup
     - ☑ Inno Setup Preprocessor
     - ☑ Help files
   - Click **"Next"**

6. **Select Start Menu Folder**:
   - Default: "Inno Setup 6"
   - Click **"Next"**

7. **Select Additional Tasks**:
   - ☑ **Create a desktop icon** (optional)
   - ☑ **Associate .iss files with Inno Setup**
   - Click **"Next"**

8. **Ready to Install**:
   - Click **"Install"**

9. **Completing Setup**:
   - Click **"Finish"**

---

## ✅ Verify Installation

### Check if Inno Setup is installed:

**Method 1: Check Program Files**
```
C:\Program Files (x86)\Inno Setup 6\ISCC.exe
```

**Method 2: Run from Command Prompt**
```bash
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
```

If you see compiler version info, it's installed correctly!

---

## 🔧 Your Project is Already Configured!

Your DetectIt project already has the Inno Setup script ready:

### Configuration File: `installer-script.iss`

**Already configured with:**
- ✅ App name: DetectIt
- ✅ Version: 1.0.0
- ✅ Publisher: DetectIt Team
- ✅ Installation directory
- ✅ Start Menu shortcuts
- ✅ Desktop shortcut option
- ✅ Uninstaller
- ✅ License agreement
- ✅ README display

---

## 🚀 Build the Installer

### Once Inno Setup is installed:

1. **Make sure you have**:
   - ✅ Inno Setup installed
   - ✅ `app.ico` created (convert logo.png)
   - ✅ .NET 8.0 SDK installed

2. **Open Command Prompt** in project folder:
   ```bash
   cd C:\Users\Aniket\HardwareDetector
   ```

3. **Run the build script**:
   ```bash
   create-installer.bat
   ```

4. **Wait** for the build (2-3 minutes)

5. **Get your installer**:
   ```
   Output\DetectIt_Setup_v1.0.0.exe
   ```

---

## 📋 What the Build Script Does

`create-installer.bat` performs these steps:

1. ✅ Checks if Inno Setup is installed
2. ✅ Builds the DetectIt application with .NET
3. ✅ Copies files to `publish-installer` folder
4. ✅ Calls Inno Setup compiler (ISCC.exe)
5. ✅ Reads `installer-script.iss` configuration
6. ✅ Creates installer with your logo
7. ✅ Outputs to `Output\DetectIt_Setup_v1.0.0.exe`

---

## 🎯 Alternative: Manual Build

If you prefer to build manually:

### Using Inno Setup Compiler GUI:

1. **Open Inno Setup** (from Start Menu)

2. **File** → **Open**

3. **Select**: `C:\Users\Aniket\HardwareDetector\installer-script.iss`

4. **Build** → **Compile**

5. **Wait** for compilation

6. **Output**: Shows in the Compiler Output window

---

## 🔍 Troubleshooting

### "Inno Setup not found" error:

**Fix 1**: Add to PATH
```bash
set PATH=%PATH%;C:\Program Files (x86)\Inno Setup 6
```

**Fix 2**: Edit `create-installer.bat`

Find this section:
```batch
if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    set ISCC="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
```

Update the path to match your installation location.

---

### Build fails with "File not found":

**Missing files checklist:**
- [ ] `app.ico` - Convert logo.png first
- [ ] `LICENSE.rtf` - Already created ✅
- [ ] `README.rtf` - Already created ✅
- [ ] `publish-installer\DetectIt.exe` - Created during build

---

### Icon not showing:

1. Verify `app.ico` exists in project root
2. Check file size (should be 50-200 KB)
3. Rebuild: `create-installer.bat`

---

## 📁 Project Structure After Build

```
HardwareDetector/
│
├── 📄 Source Files
│   ├── MainForm.cs
│   ├── ProgramGUI.cs
│   └── HardwareDetectorApp.csproj
│
├── 🎨 Resources
│   ├── logo.png           ← Your logo
│   ├── app.ico            ← Convert from logo.png
│   ├── app.manifest
│   ├── LICENSE.rtf
│   └── README.rtf
│
├── 🔧 Installer Config
│   └── installer-script.iss  ← Inno Setup script
│
├── 🏗️ Build Output
│   ├── publish-installer/  ← Build files (temporary)
│   └── Output/
│       └── DetectIt_Setup_v1.0.0.exe  ← Final installer! 🎉
│
└── 📖 Documentation
    ├── QUICK-START.md
    ├── COMPLETE-INSTALLATION-GUIDE.md
    └── ICON-CREATION-GUIDE.md
```

---

## ⚙️ Customize Installer (Optional)

Edit `installer-script.iss` to customize:

### Change Version:
```ini
AppVersion=1.0.0  ← Change this
```

### Change Install Location:
```ini
DefaultDirName={autopf}\DetectIt  ← Change folder name
```

### Change Publisher:
```ini
AppPublisher=DetectIt Team  ← Your name/company
```

### Add/Remove Shortcuts:
```ini
[Tasks]
Name: "desktopicon"; Description: "Create desktop icon"  ← Edit or remove
```

After changes, rebuild with `create-installer.bat`

---

## 🎉 Summary

### Installation Checklist:

- [ ] Download Inno Setup from https://jrsoftware.org/isdl.php
- [ ] Install Inno Setup (use default options)
- [ ] Verify installation at `C:\Program Files (x86)\Inno Setup 6\`
- [ ] Convert `logo.png` to `app.ico`
- [ ] Run `create-installer.bat`
- [ ] Get installer from `Output\DetectIt_Setup_v1.0.0.exe`

---

## 📞 Need Help?

### Inno Setup Resources:
- **Official Site**: https://jrsoftware.org/isinfo.php
- **Documentation**: https://jrsoftware.org/ishelp/
- **Examples**: Included with Inno Setup installation

### DetectIt Project:
- Check `create-installer.bat` output for specific errors
- Run `quick-test.bat` first to verify .NET build works
- See `QUICK-START.md` for step-by-step guide

---

**You're ready to create a professional Windows installer!** 🚀

**Estimated setup time**: 10 minutes
**Build time**: 2-3 minutes per build
