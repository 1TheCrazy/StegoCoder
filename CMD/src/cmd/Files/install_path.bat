@echo off
setlocal enabledelayedexpansion

set "NEW_PATH=C:\Scripts\1TheCrazy\StegoCoder"

for /f "tokens=2*" %%a in ('reg query "HKCU\Environment" /v Path') do set "CURRENT_PATH=%%b"

echo %CURRENT_PATH% | findstr /i /c:"%NEW_PATH%" >nul
if %errorlevel%==0 (
    goto :EOF
)

set "UPDATED_PATH=%CURRENT_PATH%;%NEW_PATH%"

reg add "HKCU\Environment" /v Path /t REG_EXPAND_SZ /d "%UPDATED_PATH%" /f

endlocal

exit /b 0
