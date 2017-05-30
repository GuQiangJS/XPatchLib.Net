@echo off

FOR %%b in (20,35,40
    ) do (
	cd c:
	call "%~dp0build.bat" %%b debug false ErrorOnly;Summary
	call "%~dp0unittest.bat" %%b
	call "%~dp0build.bat" %%b release true Summary;PerformanceSummary
)