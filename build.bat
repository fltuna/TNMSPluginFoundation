@echo off
setlocal enabledelayedexpansion

REM Parse command line arguments
set PLATFORM=win-x64
set PLATFORM_NAME=Windows
if /I "%1"=="linux" (
    set PLATFORM=linux-x64
    set PLATFORM_NAME=Linux
) else if /I "%1"=="windows" (
    set PLATFORM=win-x64
    set PLATFORM_NAME=Windows
) else if not "%1"=="" (
    echo Invalid platform: %1
    echo Usage: build.bat [windows^|linux]
    echo   windows - Build for Windows ^(default^)
    echo   linux   - Build for Linux
    exit /b 1
)

title Building TNMS Projects for %PLATFORM_NAME%
rd ".build/gamedata" /S /Q
rd ".build/modules" /S /Q
rd ".build/shared" /S /Q
cls

echo Building TNMS Projects for %PLATFORM_NAME% (%PLATFORM%)
echo:

REM Define projects to build (add/remove projects as needed)
set PROJECTS=TnmsAdministrationPlatform TnmsCentralizedDbPlatform TnmsExtendableTargeting TnmsLocalizationPlatform TnmsPluginFoundation.Example A0TnmsDependencyLoader
set SHARED_PROJECTS=TnmsAdministrationPlatform.Shared TnmsCentralizedDbPlatform.Shared TnmsExtendableTargeting.Shared TnmsLocalizationPlatform.Shared TnmsPluginFoundation

REM Define DLLs to remove (provided by ModSharp)
set DLLS_TO_REMOVE=Google.Protobuf.dll McMaster.NETCore.Plugins.dll Microsoft.Extensions.Configuration.dll Microsoft.Extensions.Configuration.Abstractions.dll Microsoft.Extensions.Configuration.Binder.dll Microsoft.Extensions.Configuration.FileExtensions.dll Microsoft.Extensions.Configuration.Json.dll Microsoft.Extensions.DependencyInjection.dll Microsoft.Extensions.DependencyInjection.Abstractions.dll Microsoft.Extensions.Diagnostics.dll Microsoft.Extensions.Diagnostics.Abstractions.dll Microsoft.Extensions.FileProviders.Abstractions.dll Microsoft.Extensions.FileProviders.Physical.dll Microsoft.Extensions.FileSystemGlobbing.dll Microsoft.Extensions.Http.dll Microsoft.Extensions.Logging.dll Microsoft.Extensions.Logging.Abstractions.dll Microsoft.Extensions.Logging.Configuration.dll Microsoft.Extensions.Logging.Console.dll Microsoft.Extensions.Options.dll Microsoft.Extensions.Options.ConfigurationExtensions.dll Microsoft.Extensions.Primitives.dll Serilog.dll Serilog.Extensions.Logging.dll Serilog.Sinks.Console.dll Serilog.Sinks.File.dll Serilog.Sinks.Async.dll Serilog.Expressions.dll System.Text.Json

REM Define Shared DLLs to remove from TnmsPluginFoundation.Example (these are provided by shared directory)
set SHARED_DLLS_TO_REMOVE=TnmsAdministrationPlatform.Shared.dll TnmsExtendableTargeting.Shared.dll TnmsLocalizationPlatform.Shared.dll TnmsCentralizedDbPlatform.Shared.dll

echo Building shared projects...
for %%P in (%SHARED_PROJECTS%) do (
    if exist "%%P\%%P.csproj" (
        echo Building shared project: %%P
        dotnet build %%P/%%P.csproj -f net9.0 -r %PLATFORM% --disable-build-servers -c Release --no-dependencies
        dotnet publish %%P/%%P.csproj -f net9.0 -r %PLATFORM% --disable-build-servers --no-self-contained -c Release --no-build --output ".build/shared/%%P"
        
        echo Removing DLLs that already present in ModSharp from %%P...
        for %%D in (%DLLS_TO_REMOVE%) do (
            if exist ".build\shared\%%P\%%D" (
                del ".build\shared\%%P\%%D" /Q
            )
        )
        
        REM Move all DLLs except the main shared DLL to dependencies directory
        set MAIN_DLL=%%P.dll
        set MODULE_DIR=.build\shared\%%P
        set DEP_DIR=!MODULE_DIR!\dependencies
        set HAS_DEPENDENCIES=0
        
        REM Check if there are any dependency DLLs to move
        for %%F in (!MODULE_DIR!\*.dll) do (
            if /I not "%%~nxF"=="!MAIN_DLL!" (
                set HAS_DEPENDENCIES=1
            )
        )
        
        REM Only create dependencies directory and move files if dependencies exist
        if !HAS_DEPENDENCIES! EQU 1 (
            if not exist "!DEP_DIR!" (
                mkdir "!DEP_DIR!"
            )
            
            for %%F in (!MODULE_DIR!\*.dll) do (
                if /I not "%%~nxF"=="!MAIN_DLL!" (
                    move "%%F" "!DEP_DIR!\"
                )
            )
        )
    ) else (
        echo Warning: %%P.csproj not found, skipping...
    )
)

echo:
echo Building main projects...
for %%P in (%PROJECTS%) do (
    if exist "%%P\%%P.csproj" (
        echo Building project: %%P
        dotnet build %%P/%%P.csproj -f net9.0 -r %PLATFORM% --disable-build-servers -c Release --no-dependencies
        dotnet publish %%P/%%P.csproj -f net9.0 -r %PLATFORM% --disable-build-servers --no-self-contained -c Release --no-build --output ".build/modules/%%P"
        
        echo Removing DLLs that already present in ModSharp from %%P...
        for %%D in (%DLLS_TO_REMOVE%) do (
            if exist ".build\modules\%%P\%%D" (
                del ".build\modules\%%P\%%D" /Q
            )
        )
        
        REM Special handling for TnmsPluginFoundation.Example - remove shared DLLs
        if "%%P"=="TnmsPluginFoundation.Example" (
            echo Removing Shared DLLs from TnmsPluginFoundation.Example...
            for %%S in (%SHARED_DLLS_TO_REMOVE%) do (
                if exist ".build\modules\%%P\%%S" (
                    echo Removing %%S from %%P
                    del ".build\modules\%%P\%%S" /Q
                )
            )
        )
        
        REM Move all DLLs except the main plugin DLL to dependencies directory
        set MAIN_DLL=%%P.dll
        set MODULE_DIR=.build\modules\%%P
        set DEP_DIR=!MODULE_DIR!\dependencies
        set HAS_DEPENDENCIES=0
        
        REM Check if there are any dependency DLLs to move
        for %%F in (!MODULE_DIR!\*.dll) do (
            if /I not "%%~nxF"=="!MAIN_DLL!" (
                set HAS_DEPENDENCIES=1
            )
        )
        
        REM Only create dependencies directory and move files if dependencies exist
        if !HAS_DEPENDENCIES! EQU 1 (
            if not exist "!DEP_DIR!" (
                mkdir "!DEP_DIR!"
            )
            
            for %%F in (!MODULE_DIR!\*.dll) do (
                if /I not "%%~nxF"=="!MAIN_DLL!" (
                    move "%%F" "!DEP_DIR!\"
                )
            )
        )
        
        echo Renaming appsettings.json for %%P...
        if exist ".build\modules\%%P\appsettings.json" move ".build\modules\%%P\appsettings.json" ".build\modules\%%P\appsettings.example.json"
                
        echo Copying lang files for %%P...
        if exist "%%P\lang\" xcopy "%%P\lang\*" ".build/modules/%%P/lang/" /E /I /Y
        
        echo:
    ) else (
        echo Warning: %%P.csproj not found, skipping...
    )
)

echo Copying GameData...
if exist "gamedata\" xcopy "gamedata\*" ".build/gamedata/" /E /I /Y

echo:
echo Build and copy completed for all projects.

REM Copy to ModSharp directory if MOD_SHARP_DIR is set
if "%MOD_SHARP_DIR%"=="" (
    echo MOD_SHARP_DIR environment variable is not set. Skipping ModSharp copy.
    echo To enable ModSharp copy, set MOD_SHARP_DIR environment variable to your ModSharp installation path.
) else (
    echo:
    echo Copying to ModSharp directory: %MOD_SHARP_DIR%
    
    echo Copying shared projects to ModSharp...
    for %%P in (%SHARED_PROJECTS%) do (
        if exist ".build\shared\%%P\" (
            echo Copying %%P to ModSharp shared directory...
            xcopy ".build\shared\%%P\*" "%MOD_SHARP_DIR%\shared\%%P\" /E /I /Y
        )
    )
    
    echo Copying main projects to ModSharp...
    for %%P in (%PROJECTS%) do (
        if exist ".build\modules\%%P\" (
            echo Copying %%P to ModSharp modules directory...
            xcopy ".build\modules\%%P\*" "%MOD_SHARP_DIR%\modules\%%P\" /E /I /Y
        )
    )
    
    echo Copying GameData to ModSharp...
    if exist ".build\gamedata\" xcopy ".build\gamedata\*" "%MOD_SHARP_DIR%\gamedata\" /E /I /Y
    
    echo:
    echo Successfully copied all projects to ModSharp directory.
)

echo: