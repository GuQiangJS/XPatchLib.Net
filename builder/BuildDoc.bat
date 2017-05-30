@echo off

::version Exp:40

set "versionName=Net%1"
set "framwork=framework%1"
set "logFilePath=%~dp0BuildLogs\"
set "srcPath=%~dp0..\src"
set "t=Clean;Rebuild"

FOR %%b in (
       "%VS140COMNTOOLS%..\..\VC\vcvarsall.bat"
       "%VS120COMNTOOLS%..\..\VC\vcvarsall.bat" 
       "%VS110COMNTOOLS%..\..\VC\vcvarsall.bat"
       "%VS90COMNTOOLS%..\..\VC\vcvarsall.bat" 
    ) do (
    if exist %%b (
       call %%b
       goto build
    )
)

::https://social.msdn.microsoft.com/Forums/en-US/1071be0e-2a46-4c30-9546-ea9d7c4755fa/where-is-vcvarsallbat-file?forum=visualstudiogeneral
echo "not found vcvarshall.bat"

:build
(
FOR %%c in (
	CHMBuilder,MarkdownBuilder,WebBuilder ) do (
		if not exist %logFilePath% md %logFilePath%
		
		msbuild "%srcPath%\HelperBuilder\%%c\%%c.%versionName%.shfbproj" /t:%t% /l:FileLogger,Microsoft.Build.Engine;logfile=%logFilePath%%%c_%versionName%.log;Encoding=UTF-8 /p:Configuration=Release;SolutionDir=%srcPath%\ /clp:ErrorOnly;Summary;PerformanceSummary
	)
)

IF %ERRORLEVEL% EQU 0 (echo ** Build completed.) ELSE (echo %ERRORLEVEL% PAUSE)