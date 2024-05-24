@echo off

>nul 2>&1 "%SYSTEMROOT%\system32\net.exe" session
if %errorlevel% neq 0 (
    powershell -command "Start-Process cmd -ArgumentList '/c, %~dpnx0' -Verb runAs"
    exit /b
)

SETX PATH "%PATH:C:\Scripts\1TheCrazy\StenoCoder;=%" /M

pause
