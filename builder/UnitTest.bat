@echo off

::version Exp:40

set "assemblyName=XPatchLib.UnitTest"
set "versionName=Net%1"
set "currentpath=%~dp0"
set "srcPath=%~dp0..\src"
set "testResults=%~dp0TestResults"

set "trx2html=%~dp0trx2hml_0.7.3\trx2html.exe"

FOR %%b in (
       "%VS140COMNTOOLS%..\IDE\CommonExtensions\Microsoft\TestWindow"
       "%VS120COMNTOOLS%..\IDE\CommonExtensions\Microsoft\TestWindow"
       "%VS110COMNTOOLS%..\IDE\CommonExtensions\Microsoft\TestWindow"
       "%VS90COMNTOOLS%..\IDE\CommonExtensions\Microsoft\TestWindow"
    ) do (
    if exist %%b (
	set "testPath=%%b"
    )
)

if not exist %testResults% md %testResults%

::任务管理器-修改日期、时间或数字格式-短日期格式：dddd M/d/yyyy （其中dddd表示day of week）
::或者不改系统设置的话，根据%date%回显对变量位置进行相应的调整，比如回显是‘2015/3/11’，YEAR就是%date:~0,4%。

set "testResultFile=%~dp0TestResults\%assemblyName%_%versionName% %DATE:~0,4%-%DATE:~5,2%-%DATE:~8,2% %TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%.trx"

%testPath%\..\..\..\MSTest.exe /testcontainer:%srcPath%\%assemblyName%\bin\debug\%versionName%\%assemblyName%.dll /resultsfile:"%testResultFile%"
::%testPath%\vstest.console.exe "%srcPath%\%assemblyName%\bin\debug\%versionName%\%assemblyName%.dll" /logger:trx /Platform:x86

call %trx2html% "%testResultFile%"

call "%testResultFile%".htm