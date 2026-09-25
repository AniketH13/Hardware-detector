# DetectIt - Complete Installation Steps

## 📋 What Was Done

### ✅ Application Rebranded to "DetectIt"
All files have been updated with the new name:
- Application title: **DetectIt**
- Window title: **DetectIt - Hardware Detector**
- Installer name: **DetectIt_Setup_v1.0.0.exe**
- Company: **DetectIt Team**

### ✅ Removed Files (Console Version)
- ❌ `Program.cs` (console app - removed)
- ❌ `HardwareDetector.csproj` (console project - removed)
- ❌ `build-exe.bat` (console builder - removed)

---

## 🚀 Installation Steps for End Users

### **How to Install DetectIt:**

1. **Download the installer**
   - File: `DetectIt_Setup_v1.0.0.exe`

2. **Run the installer**
   - Double-click `DetectIt_Setup_v1.0.0.exe`
   - Windows may show "Windows protected your PC"
   - Click "More info" → "Run anyway"

3. **Installation Wizard Opens**
   
   **Step 1: Welcome Screen**
   - Click **"Next"**

   **Step 2: License Agreement**
   - Read the MIT License
   - Select **"I accept the agreement"**
   - Click **"Next"**

   **Step 3: Select Destination Location**
   - Default: `C:\Program Files\DetectIt`
   - Or click **"Browse..."** to choose custom location
   - Click **"Next"**

   **Step 4: Select Start Menu Folder**
   - Default: `DetectIt`
   - Click **"Next"**

   **Step 5: Select Additional Tasks**
   - ☑ **Create a desktop icon** (recommended)
   - ☑ **Create a Quick Launch icon** (optional)
   - Click **"Next"**

   **Step 6: Ready to Install**
   - Review your choices
   - Click **"Install"**

   **Step 7: Installing**
   - Progress bar shows installation
   - Takes 10-30 seconds

   **Step 8: Completing Setup**
   - ☑ **Launch DetectIt** (optional)
   - Click **"Finish"**

4. **DetectIt is now installed!**
   - Find it in Start Menu → DetectIt
   - Or double-click desktop shortcut

---

## 🖥️ Using DetectIt

1. **Launch the application**
   - From Start Menu or Desktop shortcut
   - Application will request Administrator privileges (click Yes for full hardware access)

2. **View hardware information**
   - Hardware automatically scans on startup
   - Tree view on left shows categories:
     - 🖥️ CPU Information
     - 💾 Memory Information
     - 🔌 Motherboard Information
     - 🎮 GPU Information
     - 💿 Disk Information
     - 🌐 Network Adapters
     - 🖥️ Operating System

3. **Click any category** to see detailed information in the right panel

4. **Use the buttons:**
   - **Refresh**: Rescan all hardware
   - **Export**: Save information to a text file

---

## 🗑️ How to Uninstall DetectIt

### **Method 1: Windows 11**
1. Open **Settings** (Win + I)
2. Go to **Apps** → **Installed apps**
3. Search for **"DetectIt"**
4. Click **⋮** (three dots) → **Uninstall**
5. Click **"Uninstall"** again to confirm
6. Follow uninstall wizard
7. Click **"Yes"** when asked to remove DetectIt
8. Click **"OK"** when complete

### **Method 2: Windows 10**
1. Open **Control Panel**
2. Click **Programs** → **Programs and Features**
3. Find **"DetectIt"** in the list
4. Right-click → **Uninstall**
5. Follow uninstall wizard
6. Click **"OK"** when complete

### **Method 3: Start Menu**
1. Open **Start Menu**
2. Find **"DetectIt"** folder
3. Click **"Uninstall DetectIt"**
4. Follow uninstall wizard

**Note:** All files and settings will be completely removed.

---

## 🛠️ For Developers: Building the Installer

### **Prerequisites:**
1. **.NET 8.0 SDK** - https://dotnet.microsoft.com/download/dotnet/8.0
2. **Inno Setup** - https://jrsoftware.org/isdl.php
3. **app.ico** - Convert `icon.svg` using https://convertio.co/svg-ico/

### **Build Steps:**

1. **Test the build:**
   ```bash
   quick-test.bat
   ```
   ✅ Should show "BUILD SUCCESSFUL"

2. **Create the installer:**
   ```bash
   create-installer.bat
   ```
   ✅ Creates `Output\DetectIt_Setup_v1.0.0.exe`

3. **Clean build artifacts (optional):**
   ```bash
   cleanup.bat
   ```

### **Build Output:**
- **Installer**: `Output\DetectIt_Setup_v1.0.0.exe`
- **Size**: ~75-85 MB (includes .NET runtime)
- **Target**: Windows 10/11 (64-bit)

---

## 📦 What Gets Installed

### **Installation Locations:**

**Program Files:**
- `C:\Program Files\DetectIt\DetectIt.exe`
- Plus .NET runtime libraries

**Start Menu:**
- `Start Menu\Programs\DetectIt\DetectIt.exe`
- `Start Menu\Programs\DetectIt\Uninstall DetectIt`

**Desktop (if selected):**
- `Desktop\DetectIt.lnk`

**Registry:**
- `HKEY_CURRENT_USER\Software\Microsoft\DetectIt`
- Uninstall entry in Windows Apps & Features

---

## 🔒 Security & Permissions

### **Why Administrator Privileges?**
DetectIt requests Administrator privileges to access complete hardware information through Windows Management Instrumentation (WMI). Some hardware details are restricted without admin rights.

### **What DetectIt Does:**
- ✅ Reads hardware information locally
- ✅ No internet connection required
- ✅ No data sent anywhere
- ✅ No telemetry or tracking

### **What DetectIt Does NOT Do:**
- ❌ No network access
- ❌ No data collection
- ❌ No modifications to system
- ❌ No background processes

---

## 📊 System Requirements

**Minimum:**
- Windows 10 (64-bit) or Windows 11
- 100 MB free disk space
- No additional software needed

**Recommended:**
- Administrator account for full hardware access
- 150 MB free disk space

---

## ❓ Troubleshooting

### **"Windows protected your PC" message**
This is normal for unsigned applications.
- Click "More info"
- Click "Run anyway"

### **Some hardware not showing**
- Right-click DetectIt shortcut
- Select "Run as administrator"
- Try again

### **Installation fails**
- Make sure you have Administrator privileges
- Close any antivirus temporarily
- Ensure 100+ MB free disk space

### **Can't find DetectIt after install**
- Check Start Menu → type "DetectIt"
- Check installation folder: `C:\Program Files\DetectIt`

---

## 📞 Support

**Documentation:**
- `README.md` - Overview and features
- `QA-REPORT.md` - Testing details
- `PROJECT-FILES.md` - File structure

**Online:**
- GitHub: https://github.com/detectit
- Issues: https://github.com/detectit/issues

---

## ✅ Installation Checklist

**For End Users:**
- [ ] Download `DetectIt_Setup_v1.0.0.exe`
- [ ] Run installer
- [ ] Accept license agreement
- [ ] Choose installation location
- [ ] Select additional tasks (desktop icon)
- [ ] Complete installation
- [ ] Launch DetectIt
- [ ] Allow Administrator privileges
- [ ] View hardware information

**For Developers:**
- [ ] Install .NET 8.0 SDK
- [ ] Install Inno Setup
- [ ] Create `app.ico` from `icon.svg`
- [ ] Run `quick-test.bat` successfully
- [ ] Run `create-installer.bat`
- [ ] Test installer on clean Windows system
- [ ] Verify all features work
- [ ] Test uninstaller

---

**DetectIt** - Professional hardware detection made simple! 🚀
