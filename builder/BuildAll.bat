@echo off


FOR %%b in (NET20,NET35,NET40) do (
call "%~dp0build.bat" XPatchLib %%b debug false ErrorOnly;Summary
IF %ERRORLEVEL% NEQ 0 (echo %ERRORLEVEL% PAUSE)
)
	
	::call "%~dp0unittest.bat" %%b
	::call "%~dp0build.bat" %%b release true Summary;PerformanceSummary
	::call "%~dp0builddoc.bat" %%b