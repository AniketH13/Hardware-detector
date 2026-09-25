# Creating Windows Installer Packages

You have **three options** to create a proper Windows installer:

## Option 1: Inno Setup (Recommended - Easiest)

### What You Get:
- Professional `.exe` installer (like most Windows programs)
- Installation wizard with welcome screen
- Start Menu and Desktop shortcuts
- Proper uninstaller
- Appears in "Apps & Features" / "Programs and Features"

### Steps:
1. **Download Inno Setup**: https://jrsoftware.org/isdl.php
2. **Install Inno Setup** (free software)
3. **Run**: `create-installer.bat`
4. **Output**: `Output\HardwareDetector_Setup_v1.0.0.exe`

### Features:
- User-friendly installation wizard
- License agreement screen
- Choose installation directory
- Create desktop shortcut (optional)
- Start Menu entry
- Clean uninstaller
- Professional look and feel

---

## Option 2: WiX Toolset (MSI Package)

### What You Get:
- Standard `.msi` installer (Windows Installer format)
- Enterprise-grade installer
- Group Policy deployment compatible
- Corporate environment friendly

### Steps:
1. **Download WiX Toolset**: https://wixtoolset.org/releases/
2. **Install WiX** and add to PATH
3. **Run**: `build-msi.bat`
4. **Output**: `HardwareDetector_Setup.msi`

### Features:
- Microsoft standard installer format
- Can be deployed via Group Policy
- Silent installation support
- Enterprise deployment ready

---

## Option 3: ClickOnce (Simple Deployment)

### What You Get:
- `.application` file for easy deployment
- Auto-update capable
- Web deployment friendly

### Steps:
1. Open project in Visual Studio
2. Right-click project → Properties
3. Click "Publish" tab
4. Configure and publish

---

## Recommendation

**Use Option 1 (Inno Setup)** if you want:
- The easiest solution
- Professional-looking installer
- Standard `.exe` file that anyone can run
- No technical knowledge required

The resulting installer will look and behave like any professional Windows application installer!

---

## After Creating Installer

You can distribute:
- `HardwareDetector_Setup_v1.0.0.exe` (Inno Setup) or
- `HardwareDetector_Setup.msi` (WiX)

Users just double-click the file and follow the installation wizard!

---

## Customization

You can customize:
- Company name
- Product version
- Installation icons
- License agreement
- Welcome screen images
- Installation directory

Edit the `installer-script.iss` file (for Inno Setup) or `installer.wxs` (for WiX) to customize.
