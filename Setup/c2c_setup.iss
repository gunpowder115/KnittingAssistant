 ;---------------------------------------------------------------------------
 ;
 ;              Установочный скрипт для C2C Knitting Assistant
 ;
 ;---------------------------------------------------------------------------

 ;---------------------------------------------------------------------------
 ;              Определяем некоторые константы
 ;---------------------------------------------------------------------------

 ; Имя приложения
 #define Name "C2C Knitting Assistant"
 ; Версия приложения
 #define Version "1.0.0"
 ; Фирма-разработчик
 #define Publisher "Владислав Порохин"
 ; Сайт фирмы-разработчика
 #define URL "https://vk.com/vlad_poroh"
 ; Имя исполняемого модуля
 #define ExeName "KnittingAssistant.exe"

 ;---------------------------------------------------------------------------
 ; Параметры установки
 ;---------------------------------------------------------------------------
 [Setup]
 ; Уникальный идентификатор приложения (Tools->Generage GUID)
 AppId={{7E608C85-F913-4A49-8149-21CA9CB0A38A}

 ; Прочая инфа, отображаемая при установке
 AppName={#Name}
 AppVersion={#Version}
 AppPublisher={#Publisher}
 AppPublisherURL={#URL}
 AppSupportURL={#URL}
 AppUpdatesURL={#URL}

 ; Путь установки по умолчанию
 DefaultDirName={commonpf}\{#Name}
 ; Имя группы в меню Пуск
 DefaultGroupName={#Name}

 ; Каталог, куда будет записан собранный setup и имя исполняемого файла
 OutputDir=D:\Development\KnittingAssistant\Setup
 OutputBaseFilename=c2c_setup

 ; Файл иконки
 SetupIconFile=D:\Development\KnittingAssistant\KnittingAssistant\Resources\Icons\large_app_icon.ico

 ; Параметры сжатия
 Compression=lzma
 SolidCompression=yes

 ;---------------------------------------------------------------------------
 ; Устанавливаем языки для процесса установки
 ;---------------------------------------------------------------------------
 [Languages]
 Name: "english"; MessagesFile: "compiler:Default.isl"; LicenseFile: "D:\Development\KnittingAssistant\Licenses\License_ENG.txt" 
 Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"; LicenseFile: "D:\Development\KnittingAssistant\Licenses\License_RUS.txt"

 ;---------------------------------------------------------------------------
 ; Опционально - некоторые задачи, которые надо выполнить при установке
 ;---------------------------------------------------------------------------
 [Tasks]
 ; создание иконки на рабочем столе
 Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

 ;---------------------------------------------------------------------------
 ; Файлы, которые надо включить в пакет установщика
 ;---------------------------------------------------------------------------
 [Files]

 ; Исполняемый файл
 Source: "D:\Development\KnittingAssistant\KnittingAssistant\bin\Release\net5.0-windows\KnittingAssistant.exe"; DestDir: "{app}"; Flags: ignoreversion
 ; Прилагающиеся ресурсы
 Source: "D:\Development\KnittingAssistant\KnittingAssistant\bin\Release\net5.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
 ; .NET 5.0
 Source: "D:\Installers\.NET\windowsdesktop-runtime-5.0.16-win-x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: not IsRequiredDotNetDetected

 ;---------------------------------------------------------------------------
 ; Указываем установщику, где он должен взять иконки
 ;---------------------------------------------------------------------------
 [Icons]

 Name: "{group}\{#Name}"; Filename: "{app}\{#ExeName}"
 Name: "{commondesktop}\{#Name}"; Filename: "{app}\{#ExeName}"; Tasks: desktopicon

 ;---------------------------------------------------------------------------
 ; Секция кода, включённая из отдельного файла
 ;---------------------------------------------------------------------------
 [Code]
 #include "dotnet.pas"

 [Run]
 ;---------------------------------------------------------------------------
 ; Секция запуска после инсталляции
 ;---------------------------------------------------------------------------
 Filename: {tmp}\windowsdesktop-runtime-5.0.16-win-x64.exe; Parameters: "/q:a /c:""install /l /q"""; Check: not IsRequiredDotNetDetected; StatusMsg: Microsoft .NET 5.0 is installed. Please wait... 
