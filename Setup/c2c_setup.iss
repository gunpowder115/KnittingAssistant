 ;---------------------------------------------------------------------------
 ;
 ;              ������������ ������ ��� C2C Knitting Assistant
 ;
 ;---------------------------------------------------------------------------

 ;---------------------------------------------------------------------------
 ;              ���������� ��������� ���������
 ;---------------------------------------------------------------------------

 ; ��� ����������
 #define Name "C2C Knitting Assistant"
 ; ������ ����������
 #define Version "1.0.0"
 ; �����-�����������
 #define Publisher "��������� �������"
 ; ���� �����-������������
 #define URL "https://vk.com/vlad_poroh"
 ; ��� ������������ ������
 #define ExeName "KnittingAssistant.exe"

 ;---------------------------------------------------------------------------
 ; ��������� ���������
 ;---------------------------------------------------------------------------
 [Setup]
 ; ���������� ������������� ���������� (Tools->Generage GUID)
 AppId={{7E608C85-F913-4A49-8149-21CA9CB0A38A}

 ; ������ ����, ������������ ��� ���������
 AppName={#Name}
 AppVersion={#Version}
 AppPublisher={#Publisher}
 AppPublisherURL={#URL}
 AppSupportURL={#URL}
 AppUpdatesURL={#URL}

 ; ���� ��������� �� ���������
 DefaultDirName={commonpf}\{#Name}
 ; ��� ������ � ���� ����
 DefaultGroupName={#Name}

 ; �������, ���� ����� ������� ��������� setup � ��� ������������ �����
 OutputDir=D:\Development\KnittingAssistant\Setup
 OutputBaseFilename=c2c_setup

 ; ���� ������
 SetupIconFile=D:\Development\KnittingAssistant\KnittingAssistant\Resources\Icons\large_app_icon.ico

 ; ��������� ������
 Compression=lzma
 SolidCompression=yes

 ;---------------------------------------------------------------------------
 ; ������������� ����� ��� �������� ���������
 ;---------------------------------------------------------------------------
 [Languages]
 Name: "english"; MessagesFile: "compiler:Default.isl"; LicenseFile: "D:\Development\KnittingAssistant\Licenses\License_ENG.txt" 
 Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"; LicenseFile: "D:\Development\KnittingAssistant\Licenses\License_RUS.txt"

 ;---------------------------------------------------------------------------
 ; ����������� - ��������� ������, ������� ���� ��������� ��� ���������
 ;---------------------------------------------------------------------------
 [Tasks]
 ; �������� ������ �� ������� �����
 Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

 ;---------------------------------------------------------------------------
 ; �����, ������� ���� �������� � ����� �����������
 ;---------------------------------------------------------------------------
 [Files]

 ; ����������� ����
 Source: "D:\Development\KnittingAssistant\KnittingAssistant\bin\Release\net5.0-windows\KnittingAssistant.exe"; DestDir: "{app}"; Flags: ignoreversion
 ; ������������� �������
 Source: "D:\Development\KnittingAssistant\KnittingAssistant\bin\Release\net5.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
 ; .NET 5.0 x64
 Source: "D:\Installers\.NET\windowsdesktop-runtime-5.0.16-win-x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: not IsRequiredDotNetDetected_x64
 ; .NET 5.0 x86
 Source: "D:\Installers\.NET\windowsdesktop-runtime-5.0.16-win-x86.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: not IsRequiredDotNetDetected_x86

 ;---------------------------------------------------------------------------
 ; ��������� �����������, ��� �� ������ ����� ������
 ;---------------------------------------------------------------------------
 [Icons]

 Name: "{group}\{#Name}"; Filename: "{app}\{#ExeName}"
 Name: "{commondesktop}\{#Name}"; Filename: "{app}\{#ExeName}"; Tasks: desktopicon

 ;---------------------------------------------------------------------------
 ; ������ ����, ���������� �� ���������� �����
 ;---------------------------------------------------------------------------
 [Code]
 #include "dotnet.pas"

 [Run]
 ;---------------------------------------------------------------------------
 ; ������ ������� ����� �����������
 ;---------------------------------------------------------------------------
 Filename: {tmp}\windowsdesktop-runtime-5.0.16-win-x64.exe; Parameters: "/q:a /c:""install /l /q"""; Check: not IsRequiredDotNetDetected_x64; StatusMsg: Microsoft .NET 5.0 x64 is installed. Please wait... 
 Filename: {tmp}\windowsdesktop-runtime-5.0.16-win-x86.exe; Parameters: "/q:a /c:""install /l /q"""; Check: not IsRequiredDotNetDetected_x86; StatusMsg: Microsoft .NET 5.0 x86 is installed. Please wait... 
