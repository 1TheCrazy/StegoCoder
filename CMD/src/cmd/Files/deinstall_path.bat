@echo off
setlocal 

set oldPath=%PATH%

set newPath=%oldPath:;C:\Scripts\1TheCrazy\StegoCoder=%
set newPath=%newPath:;C:\Scripts\1TheCrazy\StegoCoder;=%

reg add "HKCU\Environment" /v Path /t REG_EXPAND_SZ /d "%newPath%" /f

endlocal
exit /b 0
