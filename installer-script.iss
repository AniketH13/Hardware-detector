[Setup]
AppId={{9E2A15E6-4E0B-4EE9-B0B5-891481BC3817}
AppName=DetectIt
AppVersion=1.0.0
AppPublisher=DetectIt Team
AppPublisherURL=https://github.com/detectit
AppSupportURL=https://github.com/detectit/support
AppUpdatesURL=https://github.com/detectit/releases
DefaultDirName={autopf}\DetectIt
DefaultGroupName=DetectIt
AllowNoIcons=yes
LicenseFile=LICENSE.rtf
InfoBeforeFile=README.rtf
OutputDir=Output
OutputBaseFilename=DetectIt_Setup_v1.0.0
SetupIconFile=app.ico
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
UninstallDisplayIcon={app}\HardwareDetectorApp.exe
UninstallDisplayName=DetectIt - Hardware Detector

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode

[Files]
Source: "publish-installer\HardwareDetectorApp.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish-installer\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\DetectIt"; Filename: "{app}\HardwareDetectorApp.exe"
Name: "{group}\{cm:ProgramOnTheWeb,DetectIt}"; Filename: "https://github.com/detectit"
Name: "{group}\{cm:UninstallProgram,DetectIt}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\DetectIt"; Filename: "{app}\HardwareDetectorApp.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\DetectIt"; Filename: "{app}\HardwareDetectorApp.exe"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\HardwareDetectorApp.exe"; Description: "{cm:LaunchProgram,DetectIt}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}"
