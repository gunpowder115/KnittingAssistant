//---------------------------------------------------------------------
// Проверка наличия .NET
//---------------------------------------------------------------------
function HasDotNet() : boolean;
var
  runtimes: TArrayOfString;
  registryKey: string;
begin
  registryKey := 'SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.NETCore.App';
  if (not IsWin64) then
    registryKey := 'SOFTWARE\dotnet\Setup\InstalledVersions\x86\sharedfx\Microsoft.NETCore.App';

  if not RegGetValueNames(HKLM, registryKey, runtimes) then
  begin
    Result := False;
  end
  else
  begin
    Result := True;
  end;
end;

//---------------------------------------------------------------------
// Проверка наличия нужной версии .NET
//---------------------------------------------------------------------
function IsDotNetDetected(dotNetName: string): boolean;
var
	cmd, args: string; //cmd и её аргументы
	filename: string; //временный файл для хранения результата команды
	output: AnsiString; //строка для содержимого файла
	command: string; //команда для получения списка установленных версий .NET
	resultCode: Integer; //результат выполнения команды	
begin
	filename := ExpandConstant('{tmp}\dotnet.txt');
	cmd := ExpandConstant('{cmd}');
	command := 'dotnet --list-runtimes';
	args := '/C ' + command + ' > "' + filename + '" 2>&1';
  if HasDotNet() then //в системе установлен .NET
  begin
    if Exec(cmd, args, '', SW_HIDE, ewWaitUntilTerminated, resultCode) and
      (resultCode = 0) then //выполнить команду в cmd
    begin
      if LoadStringFromFile(filename, output) then //прочитать файл в строку output
      begin
        if Pos(dotNetName, output) > 0 then //нужная версия .NET найдена
        begin
          Result := True;
        end
        else //нужная версия .NET не найдена
        begin
          Result := False;
        end;
      end
      else //чтение файла не удалось
      begin
        MsgBox('Failed to read output of "' + command + '"', mbError, MB_OK);
      end;
    end
      else //выполнение команды не удалось
    begin
      MsgBox('Failed to execute "' + command + '"', mbError, MB_OK);
      Result := False;
    end;    
  end
    else //.NET в системе отсутствует
  begin
    Result := False;
  end;
  DeleteFile(filename);
end;

//---------------------------------------------------------------------
// Функция-обертка для детектирования конкретной нужной нам версии
//---------------------------------------------------------------------
function IsRequiredDotNetDetected(): boolean;
begin
	Result := IsDotNetDetected('Microsoft.NETCore.App 5.0.');
end;

//---------------------------------------------------------------------
// Функция-обертка для детектирования версии x64
//---------------------------------------------------------------------
function IsRequiredDotNetDetected_x64(): boolean;
begin
	Result := IsDotNetDetected('Microsoft.NETCore.App 5.0.') and IsWin64;
end;

//---------------------------------------------------------------------
// Функция-обертка для детектирования версии x86
//---------------------------------------------------------------------
function IsRequiredDotNetDetected_x86(): boolean;
begin
	Result := IsDotNetDetected('Microsoft.NETCore.App 5.0.') and (not IsWin64);
end;

//---------------------------------------------------------------------
// Callback-функция, вызываемая при инициализации установки
//---------------------------------------------------------------------
function InitializeSetup(): boolean;
begin
	// Если нет тербуемой версии .NET выводим сообщение о том, что инсталлятор
	// попытается установить её на данный компьютер
	if not IsDotNetDetected('Microsoft.NETCore.App 5.0.') then
	begin
		MsgBox('{#Name} requires Microsoft .NET 5.0.'#13#13
             'The installer will attempt to install it', mbInformation, MB_OK);
	end;
	
	Result := True;
end;