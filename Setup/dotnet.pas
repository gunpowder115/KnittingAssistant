//---------------------------------------------------------------------
// �������� ������� ������ ������ .NET
//---------------------------------------------------------------------
function IsDotNetDetected(dotNetName: string): boolean;

var
	cmd, args: string; //cmd � � ���������
	filename: string; //��������� ���� ��� �������� ���������� �������
	output: AnsiString; //������ ��� ����������� �����
	command: string; //������� ��� ��������� ������ ������������� ������ .NET
	resultCode: Integer; //��������� ���������� �������
	
begin
	filename := ExpandConstant('{tmp}\dotnet.txt');
	cmd := ExpandConstant('{cmd}');
	command := 'dotnet --list-runtimes';
	args := '/C ' + command + ' > "' + filename + '" 2>&1';
	if Exec(cmd, args, '', SW_HIDE, ewWaitUntilTerminated, resultCode) and
		(resultCode = 0) then //��������� ������� � cmd
	begin
		if LoadStringFromFile(filename, output) then //��������� ���� � ������ output
		begin
			if Pos(dotNetName, output) > 0 then //������ ������ .NET �������
			begin
				result := True;
			end
			else //������ ������ .NET �� �������
			begin
				result := False;
			end;
		end
		else //������ ����� �� �������
		begin
			MsgBox('Failed to read output of "' + command + '"', mbError, MB_OK);
		end;
	end
    else //���������� ������� �� �������
	begin
		MsgBox('Failed to execute "' + command + '"', mbError, MB_OK);
		result := False;
	end;
	DeleteFile(filename);
end;

//---------------------------------------------------------------------
// �������-������� ��� �������������� ���������� ������ ��� ������
//---------------------------------------------------------------------
function IsRequiredDotNetDetected(): boolean;
begin
	result := IsDotNetDetected('Microsoft.NETCore.App 5.0.');
end;

//---------------------------------------------------------------------
// Callback-�������, ���������� ��� ������������� ���������
//---------------------------------------------------------------------
function InitializeSetup(): boolean;
begin
	// ���� ��� ��������� ������ .NET ������� ��������� � ���, ��� �����������
	// ���������� ���������� � �� ������ ���������
	if not IsDotNetDetected('Microsoft.NETCore.App 5.0.') then
	begin
		MsgBox('{#Name} requires Microsoft .NET 5.0.'#13#13
             'The installer will attempt to install it', mbInformation, MB_OK);
	end;
	
	result := true;
end;