@echo off
setlocal EnableExtensions

cd /d "%~dp0"

set "UNITY_EXE=C:\Program Files\Unity\Hub\Editor\6000.3.18f1\Editor\Unity.exe"
set "OUTPUT_DIR=%CD%\Builds\Android"
set "OUTPUT_APK=%OUTPUT_DIR%\LumaBay-0.1.2-alpha.apk"
set "LOG_FILE=%OUTPUT_DIR%\LumaBay-0.1.2-alpha-build.log"
set "REPORT_FILE=%OUTPUT_DIR%\LumaBay-0.1.2-alpha-build-report.txt"

if not exist "%UNITY_EXE%" (
    echo [ERROR] Unity 6000.3.18f1 not found:
    echo %UNITY_EXE%
    echo Install this editor version in Unity Hub or edit UNITY_EXE in this file.
    pause
    exit /b 2
)

if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"
if exist "%OUTPUT_APK%" del /q "%OUTPUT_APK%"
if exist "%LOG_FILE%" del /q "%LOG_FILE%"
if exist "%REPORT_FILE%" del /q "%REPORT_FILE%"

echo [1/3] Updating develop/0.1.2-alpha...
git pull --ff-only origin develop/0.1.2-alpha
if errorlevel 1 (
    echo [ERROR] Git pull failed. Resolve local changes/conflicts first.
    pause
    exit /b 3
)

echo [2/3] Unity will import and validate Premium Art Pack V2.
echo [3/3] Building Android ARM64 IL2CPP APK...

"%UNITY_EXE%" ^
  -batchmode ^
  -nographics ^
  -quit ^
  -buildTarget Android ^
  -projectPath "%CD%" ^
  -executeMethod LumaBay.Editor.LumaBayBuildScript.BuildAndroid ^
  -logFile "%LOG_FILE%"

set "BUILD_EXIT=%ERRORLEVEL%"
if not "%BUILD_EXIT%"=="0" (
    echo.
    echo [FAILED] Unity exited with code %BUILD_EXIT%.
    echo Unity log: %LOG_FILE%
    if exist "%REPORT_FILE%" echo Build report: %REPORT_FILE%
    echo.
    powershell -NoProfile -Command "Get-Content -Path '%LOG_FILE%' -Tail 160"
    pause
    exit /b %BUILD_EXIT%
)

if not exist "%OUTPUT_APK%" (
    echo.
    echo [FAILED] Unity returned success but APK was not found.
    echo Expected: %OUTPUT_APK%
    echo Unity log: %LOG_FILE%
    if exist "%REPORT_FILE%" echo Build report: %REPORT_FILE%
    pause
    exit /b 4
)

echo.
echo [SUCCESS] APK created:
echo %OUTPUT_APK%
for %%A in ("%OUTPUT_APK%") do echo Size: %%~zA bytes
explorer /select,"%OUTPUT_APK%"
pause
exit /b 0
