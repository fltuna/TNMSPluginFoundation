@echo off
title Building TNMS Projects
rd ".build/gamedata" /S /Q
rd ".build/modules" /S /Q
rd ".build/shared" /S /Q
cls

REM Define projects to build (add/remove projects as needed)
set PROJECTS=TnmsPluginFoundation TnmsAdministrationPlatform TnmsCentralizedDbPlatform TnmsExtendableTargeting TnmsLocalizationPlatform TnmsPluginFoundation.Example A0TnmsDependencyLoader
set SHARED_PROJECTS=TnmsAdministrationPlatform.Shared TnmsCentralizedDbPlatform.Shared TnmsExtendableTargeting.Shared TnmsLocalizationPlatform.Shared

REM Define DLLs to remove (provided by ModSharp)
set DLLS_TO_REMOVE=Google.Protobuf.dll McMaster.NETCore.Plugins.dll Microsoft.Extensions.Configuration.dll Microsoft.Extensions.Configuration.Abstractions.dll Microsoft.Extensions.Configuration.Binder.dll Microsoft.Extensions.Configuration.FileExtensions.dll Microsoft.Extensions.Configuration.Json.dll Microsoft.Extensions.DependencyInjection.dll Microsoft.Extensions.DependencyInjection.Abstractions.dll Microsoft.Extensions.Diagnostics.dll Microsoft.Extensions.Diagnostics.Abstractions.dll Microsoft.Extensions.FileProviders.Abstractions.dll Microsoft.Extensions.FileProviders.Physical.dll Microsoft.Extensions.FileSystemGlobbing.dll Microsoft.Extensions.Http.dll Microsoft.Extensions.Logging.dll Microsoft.Extensions.Logging.Abstractions.dll Microsoft.Extensions.Logging.Configuration.dll Microsoft.Extensions.Logging.Console.dll Microsoft.Extensions.Options.dll Microsoft.Extensions.Options.ConfigurationExtensions.dll Microsoft.Extensions.Primitives.dll Serilog.dll Serilog.Extensions.Logging.dll Serilog.Sinks.Console.dll Serilog.Sinks.File.dll Serilog.Sinks.Async.dll Serilog.Expressions.dll

echo Building shared projects...
for %%P in (%SHARED_PROJECTS%) do (
    if exist "%%P\%%P.csproj" (
        echo Building shared project: %%P
        dotnet publish %%P/%%P.csproj -f net9.0 -r win-x64 --disable-build-servers --no-self-contained -c Release --output ".build/shared/%%P"
        
        echo Removing DLLs that already present in ModSharp from %%P...
        for %%D in (%DLLS_TO_REMOVE%) do (
            if exist ".build\shared\%%P\%%D" (
                del ".build\shared\%%P\%%D" /Q
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
        dotnet publish %%P/%%P.csproj -f net9.0 -r win-x64 --disable-build-servers --no-self-contained -c Release --output ".build/modules/%%P"
        
        echo Removing DLLs that already present in ModSharp from %%P...
        for %%D in (%DLLS_TO_REMOVE%) do (
            if exist ".build\modules\%%P\%%D" (
                del ".build\modules\%%P\%%D" /Q
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
if "%MOD_SHARP_DIR_TEST_NOCOPY%"=="" (
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