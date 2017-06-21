@ECHO OFF

REM We need to use MSBuild 15.0 if present in order to support the new VSIX format in VS2017 and later

IF EXIST "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0" SET "VS150COMNTOOLS=%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0"
IF EXIST "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0" SET "VS150COMNTOOLS=%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0"
IF EXIST "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0" SET "VS150COMNTOOLS=%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0"

IF NOT EXIST "%VS150COMNTOOLS%\bin\MSBuild.exe" GOTO End

SET MSBUILD="%VS150COMNTOOLS%\bin\MSBuild.exe"
::SET NUGET=%CD%\SHFB\Source\.nuget\NuGet.exe
SET BuildConfig=%1
SET SRC=%~dp0..\src
SET RunCodeAnalysis=%2
SET ConsoleLoggerParameters=%3
SET LogFilePath="%~dp0BuildLogs"

IF '%BuildConfig%'=='' SET BuildConfig=Release
IF '%RunCodeAnalysis%'=='' SET RunCodeAnalysis=false
IF '%ConsoleLoggerParameters%'=='' SET ConsoleLoggerParameters=ErrorOnly;Summary

IF NOT EXIST "%LogFilePath%" MD %LogFilePath%

SET LogFile=%LogFilePath%\XPatchLib_%BuildConfig%.log

dotnet restore %SRC%\XPatchLib.sln

%MSBUILD% /nologo /v:m /m "%SRC%\XPatchLib.sln" /t:Rebuild /l:FileLogger,Microsoft.Build.Engine;logfile=%LogFile%;Encoding=UTF-8 /p:Configuration=%BuildConfig%;RunCodeAnalysis=%RunCodeAnalysis% /clp:%ConsoleLoggerParameters%

IF ERRORLEVEL 1 GOTO End

:End