@echo off
choice /c yn /m "Do you want to deinstall StegoCoder cmd-command"

if errorlevel 2 (
pause 
exit /b 0
)

echo Removing files...
rmdir /S /Q "C:\Scripts\1Thecrazy\StegoCoder"

echo Removing PATH entry...
set "CURRENT_DIR=%~dp0"
set "FILE_DIR=%CURRENT_DIR%Files\"

call "%FILE_DIR%deinstall_path.bat" >nul

if errorlevel 1 (
	echo There was an error removing the PATH entry.
)

echo Successfully removed StegoCoder from cmd. If you had any issues with this tool, please report them at https://github.com/1TheCrazy/StegoCoder/issues/new


pause
exit /b 0