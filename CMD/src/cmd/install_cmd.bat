@echo off
setlocal enabledelayedexpansion

set "ALIAS_FILE=stegocoder.bat"
set "EXE_FILE=stegocodercmd.exe"
set "TARGET_PATH=C:\Scripts\1TheCrazy\StegoCoder"
set "PATTERN=stegocodercmd-v*"

REM Find the executable file
for %%f in (*.exe) do (
    set "EXE_FILE=%%f"
)

set "CURRENT_DIR=%~dp0"
set "SOURCE_PATH=%CURRENT_DIR%%ALIAS_FILE%"
set "EXE_PATH=%CURRENT_DIR%%EXE_FILE%"

REM Extract the version from the executable file name
for %%F in ("%EXE_PATH%") do (
    set "CURRENT_VERSION=%%~nxF"
    set "CURRENT_VERSION=!CURRENT_VERSION:stegocodercmd-v=!"
    set "CURRENT_VERSION=!CURRENT_VERSION:.exe=!"
)

REM Check if the source files exist
if not exist "%SOURCE_PATH%" (
    echo The folder does not contain all contents necessary for the installation. Please download a new copy from https://github.com/1TheCrazy/StegoCoder.
    pause
    exit /b 1
)

if not exist "%EXE_PATH%" (
    echo The folder does not contain all contents necessary for the installation. Please download a new copy from https://github.com/1TheCrazy/StegoCoder.
    pause
    exit /b 1
)

REM Check if the target directory exists
if exist "%TARGET_PATH%" (

    for %%f in ("%TARGET_PATH%\%PATTERN%") do (
        set "filename=%%~nxf"
        set "version=!filename:stegocodercmd-v=!"
        set "version=!version:.exe=!"

        call :versionCompare !version! !CURRENT_VERSION!

        if !errorlevel! equ 0 (
            echo Updating an old version file: %%f
            del "%%f"
        ) else (
		echo The CMD Command has already been installed
		pause
		exit /b 0
	)
	
    )
) else (
    mkdir "%TARGET_PATH%"
)

copy "%SOURCE_PATH%" "%TARGET_PATH%" /Y
copy "%EXE_PATH%" "%TARGET_PATH%" /Y

if errorlevel 1 (
    echo Failed to copy the alias batch file. The CMD installation was not successful.
    pause
    exit /b 1
)

REM Include in path...
set "currentPath=%PATH%"
echo %currentPath% | findstr /i /c:"%TARGET_PATH%" >nul
if %errorlevel%==0 (
    echo The directory %TARGET_PATH% is already in the PATH.
) else (
    call install_path.bat
    echo The directory %TARGET_PATH% has been added to the PATH.
)

echo Installation of the CMD-Command was successful! You can now close this window...
echo https://github.com/1TheCrazy/StegoCoder
pause

exit /b 0

REM ------------------------------ Compare Function ------------------------------
:versionCompare
setlocal
set "ver1=%1"
set "ver2=%2"

for /f "tokens=1-3 delims=." %%a in ("%ver1%") do (
    set "ver1_maj=%%a"
    set "ver1_min=%%b"
    set "ver1_patch=%%c"
)
for /f "tokens=1-3 delims=." %%a in ("%ver2%") do (
    set "ver2_maj=%%a"
    set "ver2_min=%%b"
    set "ver2_patch=%%c"
)

if "%ver1_maj%" neq "%ver2_maj%" (
    if "%ver1_maj%" lss "%ver2_maj%" (endlocal & exit /b 0) else (endlocal & exit /b 1)
)
if "%ver1_min%" neq "%ver2_min%" (
    if "%ver1_min%" lss "%ver2_min%" (endlocal & exit /b 0) else (endlocal & exit /b 1)
)
if "%ver1_patch%" neq "%ver2_patch%" (
    if "%ver1_patch%" lss "%ver2_patch%" (endlocal & exit /b 0) else (endlocal & exit /b 1)
)

endlocal
exit /b 1
