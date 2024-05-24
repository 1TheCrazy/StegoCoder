@echo off
>nul 2>&1 "%SYSTEMROOT%\system32\net.exe" session
if %errorlevel% neq 0 (
    powershell -command "Start-Process cmd -ArgumentList '/c, %~dpnx0' -Verb runAs"
    exit /b
)

echo %PATH% > "C:\Scripts\1TheCrazy\StegoCoder\path_backup.txt"
SETX PATH "C:\Scripts\1TheCrazy\StegoCoder";%1 /M

exit /b 0
