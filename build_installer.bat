@echo off
setlocal
title Building The Iron Hills Setup (FRUSTAY)

echo ======================================================================
echo           Building The Iron Hills Installer (FRUSTAY)
echo ======================================================================
echo.

cd /d "%~dp0"

set "ISCC_PATH=C:\Program Files\Inno Setup 7\ISCC.exe"

if not exist "%ISCC_PATH%" (
    echo [ERROR] Inno Setup 7 compiler not found at:
    echo "%ISCC_PATH%"
    echo Please install Inno Setup 7 or update the path.
    pause
    exit /b 1
)

echo [1/4] Closing running instances of EpicBattle.exe...
taskkill /F /IM EpicBattle.exe >nul 2>&1

echo [2/4] Publishing Game (.NET 8 Win-x64 Self-Contained)...
if exist "dist\game" rd /s /q "dist\game"
dotnet publish EpicBattle.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o "dist\game" /nologo /verbosity:minimal

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] dotnet publish failed! Check the errors above.
    pause
    exit /b %ERRORLEVEL%
)

echo [3/4] Copying icon and essential assets to staging...
copy /y "app_icon.ico" "dist\game\app_icon.ico" >nul

echo [4/4] Compiling Inno Setup Package...
if not exist "installer_output" mkdir "installer_output"
"%ISCC_PATH%" "TheIronHills.iss"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Inno Setup compilation failed!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ======================================================================
echo   [SUCCESS] Game installer has been successfully built!
echo   Location: %~dp0installer_output\TheIronHills_Setup_v2.0.exe
echo ======================================================================
echo.

pause
endlocal
