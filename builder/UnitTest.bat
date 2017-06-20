@ECHO OFF

::需要使用NUnit Console 3 或以上版本。https://www.nunit.org/index.php?p=download

IF EXIST "%ProgramFiles(x86)%\NUnit.org\nunit-console" SET "NUNITCONSOLE=%ProgramFiles(x86)%\NUnit.org\nunit-console"

IF NOT EXIST "%NUNITCONSOLE%\nunit3-console.exe" GOTO End

SET NUNITCONSOLE_EXE="%NUNITCONSOLE%\nunit3-console.exe"

SET SRC=%~dp0..\src\XPatchLib.UnitTest\bin\Debug


FOR %%b in (
       NET20,NET35,NET40
    ) do (
	call %NUNITCONSOLE_EXE% "%SRC%\%%b\xpatchlib.unittest.dll" /result:"%SRC%\TEST_RESULT_%%b.XML"
)

:End