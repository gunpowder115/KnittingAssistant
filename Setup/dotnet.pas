//---------------------------------------------------------------------
// �������� ������� .NET
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
  if HasDotNet() then //� ������� ���������� .NET
  begin
    if Exec(cmd, args, '', SW_HIDE, ewWaitUntilTerminated, resultCode) and
      (resultCode = 0) then //��������� ������� � cmd
    begin
      if LoadStringFromFile(filename, output) then //��������� ���� � ������ output
      begin
        if Pos(dotNetName, output) > 0 then //������ ������ .NET �������
        begin
          Result := True;
        end
        else //������ ������ .NET �� �������
        begin
          Result := False;
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
      Result := False;
    end;    
  end
    else //.NET � ������� �����������
  begin
    Result := False;
  end;
  DeleteFile(filename);
end;

//---------------------------------------------------------------------
// �������-������� ��� �������������� ���������� ������ ��� ������
//---------------------------------------------------------------------
function IsRequiredDotNetDetected(): boolean;
begin
	Result := IsDotNetDetected('Microsoft.NETCore.App 5.0.');
end;

//---------------------------------------------------------------------
// �������-������� ��� �������������� ������ x64
//---------------------------------------------------------------------
function IsRequiredDotNetDetected_x64(): boolean;
begin
	Result := IsDotNetDetected('Microsoft.NETCore.App 5.0.') and IsWin64;
end;

//---------------------------------------------------------------------
// �������-������� ��� �������������� ������ x86
//---------------------------------------------------------------------
function IsRequiredDotNetDetected_x86(): boolean;
begin
	Result := IsDotNetDetected('Microsoft.NETCore.App 5.0.') and (not IsWin64);
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
	
	Result := True;
end;