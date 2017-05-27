@echo off

FOR %%b in (
       "%VS140COMNTOOLS%..\..\VC\vcvarsall.bat"
       "%VS120COMNTOOLS%..\..\VC\vcvarsall.bat" 
       "%VS110COMNTOOLS%..\..\VC\vcvarsall.bat"
       "%VS90COMNTOOLS%..\..\VC\vcvarsall.bat" 
    ) do (
    if exist %%b ( 
       ::call %%b x86
       ::goto build
    )
)
echo "Unable to detect suitable environment. Build not succeed."

set versionName=Net40
set logFileName=%versionName%_DEBUG.log
set MSBUILDLOGPATH=%~dp0%logFileName%
set BUILDERRORLEVEL=%ERRORLEVEL%
set "logPath=%~dp0"
set "projectPath=%~dp0..\src\XPatchLib"

:build
msbuild "%projectPath%\XPatchLib.%versionName%.csproj" /t:Clean;Rebuild /l:FileLogger,Microsoft.Build.Engine;logfile=%MSBUILDLOGPATH%;Encoding=UTF-8 /property:RunCodeAnalysis=false;SolutionDir=%~dp0..\src\

:: Pull the build summary from the log file
findstr /ir /c:".*Warning(s)" /c:".*Error(s)" /c:"Time Elapsed.*" "%MSBUILDLOGPATH%"
echo.
echo ** Build completed. Exit code: %BUILDERRORLEVEL%

exit /b %BUILDERRORLEVEL%