@echo off
setlocal
title Build Epic Battle (The Iron Hills)

echo ======================================================================
echo           Building Epic Battle - The Iron Hills
echo ======================================================================
echo.

cd /d "%~dp0"

echo [1/3] Closing running instances of EpicBattle.exe...
taskkill /F /IM EpicBattle.exe >nul 2>&1

echo [2/3] Compiling .NET 8 Release Single-File...
if exist "_build_out" rd /s /q "_build_out"
dotnet publish EpicBattle.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o "_build_out" /nologo /verbosity:minimal

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Build failed! Check the errors above.
    if exist "_build_out" rd /s /q "_build_out"
    echo.
    pause
    exit /b %ERRORLEVEL%
)

echo [3/3] Moving EpicBattle.exe to project root...
move /y "_build_out\EpicBattle.exe" "%~dp0EpicBattle.exe" >nul
if exist "_build_out\EpicBattle.pdb" move /y "_build_out\EpicBattle.pdb" "%~dp0EpicBattle.pdb" >nul
rd /s /q "_build_out"

echo.
echo ======================================================================
echo   [SUCCESS] Game executable is ready:
echo   %~dp0EpicBattle.exe
echo ======================================================================
echo.

choice /C YN /N /T 5 /D N /M "Launch EpicBattle.exe now? [Y/N] (auto-exit in 5s): "
if %ERRORLEVEL% EQU 1 (
    start "" "%~dp0EpicBattle.exe"
)

endlocal
