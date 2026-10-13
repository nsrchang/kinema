#define AppVer GetEnv("KINEMA_VERSION")
#if AppVer == ""
  #define AppVer "1.0.0"
#endif

[Setup]
AppId={{3F6B2C1E-8A4D-4C7B-9E2F-5A1D7C3B9E40}
AppName=KINEMA
AppVersion={#AppVer}
AppPublisher=KINEMA
DefaultDirName={autopf}\KINEMA
DisableProgramGroupPage=yes
OutputDir=..\dist
OutputBaseFilename=KINEMA-Setup
Compression=lzma2/max
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
ChangesAssociations=yes
PrivilegesRequired=admin
WizardStyle=modern
UninstallDisplayIcon={app}\Kinema.exe

[Tasks]
Name: "assoc"; Description: "關聯影片副檔名（在檔案總管雙擊影片用 KINEMA 播放）"
Name: "desktopicon"; Description: "建立桌面捷徑"; Flags: unchecked

[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\KINEMA"; Filename: "{app}\Kinema.exe"
Name: "{autodesktop}\KINEMA"; Filename: "{app}\Kinema.exe"; Tasks: desktopicon

[Registry]
Root: HKLM; Subkey: "Software\RegisteredApplications"; ValueType: string; ValueName: "KINEMA"; ValueData: "Software\KINEMA\Capabilities"; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities"; ValueType: string; ValueName: "ApplicationName"; ValueData: "KINEMA"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities"; ValueType: string; ValueName: "ApplicationDescription"; ValueData: "KINEMA 影片播放器（內建解碼與補幀）"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\Classes\KINEMA.mkv"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.mkv)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.mkv\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.mkv\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.mkv\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.mkv"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".mkv"; ValueData: "KINEMA.mkv"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".mkv"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.mp4"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.mp4)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.mp4\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.mp4\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.mp4\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.mp4"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".mp4"; ValueData: "KINEMA.mp4"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".mp4"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.avi"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.avi)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.avi\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.avi\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.avi\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.avi"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".avi"; ValueData: "KINEMA.avi"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".avi"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.mov"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.mov)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.mov\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.mov\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.mov\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.mov"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".mov"; ValueData: "KINEMA.mov"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".mov"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.wmv"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.wmv)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.wmv\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.wmv\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.wmv\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.wmv"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".wmv"; ValueData: "KINEMA.wmv"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".wmv"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.flv"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.flv)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.flv\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.flv\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.flv\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.flv"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".flv"; ValueData: "KINEMA.flv"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".flv"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.webm"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.webm)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.webm\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.webm\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.webm\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.webm"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".webm"; ValueData: "KINEMA.webm"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".webm"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.m4v"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.m4v)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.m4v\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.m4v\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.m4v\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.m4v"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".m4v"; ValueData: "KINEMA.m4v"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".m4v"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.ts"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.ts)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.ts\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.ts\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.ts\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.ts"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".ts"; ValueData: "KINEMA.ts"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".ts"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.m2ts"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.m2ts)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.m2ts\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.m2ts\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.m2ts\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.m2ts"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".m2ts"; ValueData: "KINEMA.m2ts"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".m2ts"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.mts"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.mts)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.mts\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.mts\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.mts\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.mts"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".mts"; ValueData: "KINEMA.mts"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".mts"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.mpg"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.mpg)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.mpg\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.mpg\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.mpg\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.mpg"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".mpg"; ValueData: "KINEMA.mpg"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".mpg"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.mpeg"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.mpeg)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.mpeg\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.mpeg\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.mpeg\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.mpeg"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".mpeg"; ValueData: "KINEMA.mpeg"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".mpeg"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.vob"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.vob)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.vob\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.vob\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.vob\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.vob"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".vob"; ValueData: "KINEMA.vob"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".vob"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.3gp"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.3gp)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.3gp\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.3gp\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.3gp\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.3gp"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".3gp"; ValueData: "KINEMA.3gp"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".3gp"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.rmvb"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.rmvb)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.rmvb\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.rmvb\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.rmvb\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.rmvb"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".rmvb"; ValueData: "KINEMA.rmvb"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".rmvb"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.rm"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.rm)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.rm\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.rm\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.rm\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.rm"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".rm"; ValueData: "KINEMA.rm"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".rm"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.asf"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.asf)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.asf\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.asf\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.asf\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.asf"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".asf"; ValueData: "KINEMA.asf"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".asf"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.ogv"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.ogv)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.ogv\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.ogv\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.ogv\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.ogv"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".ogv"; ValueData: "KINEMA.ogv"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".ogv"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.f4v"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.f4v)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.f4v\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.f4v\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.f4v\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.f4v"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".f4v"; ValueData: "KINEMA.f4v"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".f4v"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.divx"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.divx)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.divx\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.divx\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.divx\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.divx"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".divx"; ValueData: "KINEMA.divx"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".divx"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\KINEMA.hevc"; ValueType: string; ValueName: ""; ValueData: "KINEMA 影片 (.hevc)"; Flags: uninsdeletekey; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.hevc\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Kinema.exe,0"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\KINEMA.hevc\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Kinema.exe"" ""%1"""; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\.hevc\OpenWithProgids"; ValueType: string; ValueName: "KINEMA.hevc"; ValueData: ""; Flags: uninsdeletevalue; Tasks: assoc
Root: HKLM; Subkey: "Software\KINEMA\Capabilities\FileAssociations"; ValueType: string; ValueName: ".hevc"; ValueData: "KINEMA.hevc"; Tasks: assoc
Root: HKLM; Subkey: "Software\Classes\Applications\Kinema.exe\SupportedTypes"; ValueType: string; ValueName: ".hevc"; ValueData: ""

[Run]
Filename: "ms-settings:defaultapps?registeredAppMachine=KINEMA"; Description: "開啟 Windows 預設應用程式，把 KINEMA 設為預設影片播放器"; Flags: shellexec postinstall skipifsilent nowait; Tasks: assoc
Filename: "{app}\Kinema.exe"; Description: "啟動 KINEMA"; Flags: nowait postinstall skipifsilent unchecked
