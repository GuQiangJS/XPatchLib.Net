@echo off

::version Exp:40
::version Exp:Debug
::%3-RunCodeAnalysis Exp:false
::%4-clp Exp:ErrorOnly;PerformanceSummary;Summary

set "assemblyName=XPatchLib"
set "versionName=Net%1"
set "framwork=framework%1"
set "config=%2"
set "logFileName=%assemblyName%_%versionName%_%config%.log
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
echo "找不到vcvarshall.bat"

:build
(
if not exist %logFilePath% md %logFilePath%

msbuild "%srcPath%\%assemblyName%\%assemblyName%.%versionName%.csproj" /t:%t% /l:FileLogger,Microsoft.Build.Engine;logfile=%logFilePath%%logFileName%;Encoding=UTF-8 /p:Configuration=%config%;RunCodeAnalysis=%3;SolutionDir=%srcPath%\ /clp:%4
)

IF %ERRORLEVEL% EQU 0 (echo ** Build completed.) ELSE (echo %ERRORLEVEL% PAUSE)