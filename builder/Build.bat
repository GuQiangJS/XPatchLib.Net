@echo off

::version Exp:40
::version Exp:Debug
::%3-RunCodeAnalysis Exp:false
::%4-clp Exp:ErrorOnly;PerformanceSummary;Summary

::param		name					expample
::%1		assemblyName			XPatchLib
::%2		LibraryFrameworks		netstandard1.0
::%3		config					debug
::%4		RunCodeAnalysis			false
::%5		consoleloggerparameters	ErrorOnly;Summary

set "assemblyName=%1"
set "LibraryFrameworks=%2"
set "config=%3"
set "logFileName=%assemblyName%_%LibraryFrameworks%_%config%.log
set "logFilePath=%~dp0BuildLogs\"
set "srcPath=%~dp0..\src"
set "t=Clean;Rebuild"
set "RunCodeAnalysis=%4"
set "consoleloggerparameters=%5"

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
::此处需要先将 15.0 版本的 MsBuild.exe 文件所在的路径加入‘环境变量’，名称为MsBuild_15。"
if not exist "%Msbuild_15%msbuild.exe" (

echo "%Msbuild_15%msbuild.exe not found!"
echo First check whether to install "Build Tools for Visual Studio 2017"
echo Second check whether the "Visual Studio 2017 Build Tools" path is configured to "Environment Variables" with the name "Msbuild_15"
echo Build Tools for Visual Studio 2017 Download Url:https://www.visualstudio.com/downloads/

pause
goto exit
)

if not exist %logFilePath% md %logFilePath%

"%Msbuild_15%msbuild.exe" "%srcPath%\%assemblyName%\%assemblyName%.csproj" /t:%t% /l:FileLogger,Microsoft.Build.Engine;logfile=%logFilePath%%logFileName%;Encoding=UTF-8 /p:Configuration=%config%;LibraryFrameworks=%LibraryFrameworks%;RunCodeAnalysis=%RunCodeAnalysis%;SolutionDir=%srcPath%\ /clp:%consoleloggerparameters%

IF %ERRORLEVEL% EQU 0 (echo ** Build completed.) ELSE (echo %ERRORLEVEL% PAUSE)

goto exit
)

:exit