# Building HardwareDetector Standalone Executable

## Quick Build

Simply run the build script:

```bash
build-exe.bat
```

This will create a standalone `HardwareDetector.exe` in the `publish` folder.

## Manual Build

If you prefer to build manually, run:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

## Build Options

The project is configured to create a **self-contained, single-file executable** that:

- ✅ Includes the .NET runtime (no .NET installation required on target machine)
- ✅ Bundles all dependencies into a single .exe file
- ✅ Targets Windows x64 systems
- ✅ Uses ReadyToRun compilation for faster startup
- ✅ No installation required - just run the .exe

## Output

After building, you'll find:
- **File**: `publish\HardwareDetector.exe`
- **Size**: ~70-80 MB (includes entire .NET runtime)
- **Requirements**: Windows 10/11 (x64)

## Distribution

You can distribute the `HardwareDetector.exe` file directly. Users can:
1. Copy the .exe to any location
2. Double-click to run
3. No installation or .NET runtime required

## Building for Different Platforms

To build for 32-bit Windows:
```bash
dotnet publish -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true -o publish-x86
```

To build without including .NET runtime (smaller file, requires .NET 8.0 installed):
```bash
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish-framework
```

## Notes

- Administrator privileges may be required to access some hardware information
- The executable works on Windows 10/11 (x64)
- First run may take slightly longer due to decompression
