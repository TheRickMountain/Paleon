@echo off
setlocal

rem Publishes self-contained Release builds for Windows and Linux next to this script.
set "PROJECT=%~dp0Technolithic\Technolithic.csproj"

call :publish win-x64 "%~dp0PaleonWindows" || exit /b 1
call :publish linux-x64 "%~dp0PaleonLinux" || exit /b 1

echo.
echo Done: PaleonWindows, PaleonLinux
exit /b 0

:publish
echo.
echo === %~1 -^> %~2 ===
rem Start from an empty folder so files removed from the project don't linger in the build.
if exist "%~2" rmdir /s /q "%~2"
dotnet publish "%PROJECT%" -c Release -r %~1 -o "%~2" -p:PublishReadyToRun=false -p:TieredCompilation=false --self-contained
if errorlevel 1 (
    echo.
    echo Build for %~1 failed.
    exit /b 1
)
exit /b 0
