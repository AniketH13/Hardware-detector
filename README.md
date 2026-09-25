# DetectIt - Hardware Detector

A professional Windows desktop application for detecting and displaying comprehensive hardware information about your computer.

## 🎯 Features

- **CPU Information**: Name, manufacturer, cores, logical processors, clock speed, architecture
- **Memory Details**: RAM modules with capacity, speed, manufacturer, and part numbers
- **Motherboard Info**: Manufacturer, model, serial number, version
- **GPU Specifications**: Graphics cards with VRAM, resolution, refresh rate, driver version
- **Storage Drives**: Disk models, sizes, interface types, partitions
- **Network Adapters**: Active network cards with MAC addresses and speeds
- **Operating System**: Version, architecture, build number, install date

## 📦 Installation

### For Users:
1. Download `DetectIt_Setup_v1.0.0.exe`
2. Run the installer
3. Follow the installation wizard
4. Launch DetectIt from Start Menu

### For Developers:
See [INSTALLATION-GUIDE.md](INSTALLATION-GUIDE.md) for complete build instructions.

## 🚀 Quick Start

1. Launch **DetectIt** from Start Menu or Desktop shortcut
2. Hardware information loads automatically
3. Click categories in the tree view to see detailed information
4. Use **Refresh** button to rescan hardware
5. Use **Export** button to save information to a text file

## ⚙️ System Requirements

- Windows 10 or Windows 11 (64-bit)
- Administrator privileges recommended for full hardware access
- No additional software required (self-contained)

## 🛠️ Building from Source

### Prerequisites:
- .NET 8.0 SDK
- Inno Setup (for installer)

### Build Steps:
```bash
# Test build
quick-test.bat

# Build application
build-app.bat

# Create installer
create-installer.bat
```

Output: `Output\DetectIt_Setup_v1.0.0.exe`

## 📖 Documentation

- [INSTALLATION-GUIDE.md](INSTALLATION-GUIDE.md) - Complete installation and build guide
- [QA-REPORT.md](QA-REPORT.md) - QA testing report
- [PROJECT-FILES.md](PROJECT-FILES.md) - Project structure and file descriptions

## 🗑️ Uninstallation

**Windows 11:**
Settings → Apps → Installed apps → DetectIt → Uninstall

**Windows 10:**
Control Panel → Programs and Features → DetectIt → Uninstall

## 🔒 Security & Privacy

- DetectIt only reads hardware information using Windows Management Instrumentation (WMI)
- No data is sent over the network
- No telemetry or tracking
- All data stays on your computer
- Open source - you can review the code

## 📝 License

MIT License - See [LICENSE.rtf](LICENSE.rtf) for details

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📞 Support

For issues, questions, or feature requests:
- GitHub Issues: https://github.com/detectit/issues
- Documentation: Check the included .md files

## 👨‍💻 Author

DetectIt Team

## 🌟 Acknowledgments

Built with:
- .NET 8.0
- Windows Forms
- System.Management (WMI)
- Inno Setup (installer)

---

**DetectIt** - Professional hardware detection made simple.
