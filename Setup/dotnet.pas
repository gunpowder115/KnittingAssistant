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
	if Exec(cmd, args, '', SW_HIDE, ewWaitUntilTerminated, resultCode) and
		(resultCode = 0) then //выполнить команду в cmd
	begin
		if LoadStringFromFile(filename, output) then //прочитать файл в строку output
		begin
			if Pos(dotNetName, output) > 0 then //нужная версия .NET найдена
			begin
				result := True;
			end
			else //нужная версия .NET не найдена
			begin
				result := False;
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
		result := False;
	end;
	DeleteFile(filename);
end;

//---------------------------------------------------------------------
// Функция-обертка для детектирования конкретной нужной нам версии
//---------------------------------------------------------------------
function IsRequiredDotNetDetected(): boolean;
begin
	result := IsDotNetDetected('Microsoft.NETCore.App 5.0.');
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
	
	result := true;
end;