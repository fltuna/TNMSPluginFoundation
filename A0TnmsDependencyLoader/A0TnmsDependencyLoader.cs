using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Shared;

namespace TnmsDependencyLoader;

public class A0TnmsDependencyLoader: IModSharpModule
{
    public string DisplayName => "A0TnmsDependencyLoader";
    public string DisplayAuthor => "faketuna";

    
    private readonly ILogger _logger;
    private readonly string _sharpPath;
    private readonly ISharedSystem _sharedSystem;

    private readonly Dictionary<string, AssemblyInfo> _loadedDependencies = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _nativeLibraryPaths = new();
    private IModSharp? _modSharp;
    private readonly bool _hotReload;
    private string? _modulesPath;
    
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibrary(string lpFileName);
    
    [DllImport("libdl", EntryPoint = "dlopen")]
    private static extern IntPtr LoadLibraryLinux(string fileName, int flags);
    
    private const int RTLD_NOW = 2;
    
    public A0TnmsDependencyLoader(ISharedSystem sharedSystem,
        string?                  dllPath,
        string?                  sharpPath,
        Version?                 version,
        IConfiguration?          coreConfiguration,
        bool                     hotReload)
    {
        ArgumentNullException.ThrowIfNull(dllPath);
        ArgumentNullException.ThrowIfNull(sharpPath);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(coreConfiguration);
        
        _hotReload = hotReload;
        _sharpPath = sharpPath;
        _sharedSystem = sharedSystem;
        _logger = sharedSystem.GetLoggerFactory().CreateLogger<A0TnmsDependencyLoader>();
    }

    public bool Init()
    {
        if (_hotReload)
        {
            _logger.LogCritical("Hot reload is not supported by DependencyLoader module.");
            return false;
        }
        try
        {
            _modSharp = _sharedSystem.GetModSharp();
            
            if (!string.IsNullOrEmpty(_sharpPath))
            {
                _modulesPath = Path.Combine(_sharpPath, "modules");
            }
            else
            {
                var gamePath = _modSharp.GetGamePath();
                _modulesPath = Path.Combine(gamePath, "..", "sharp", "modules");
            }

            _logger.LogInformation($"Starting dependency loader for modules directory: {_modulesPath}");
            
            LoadAllModuleDependencies();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to initialize DependencyLoader module: {ex.Message}");
            return false;
        }
    }

    public void PostInit()
    {
        _logger.LogInformation("DependencyLoader PostInit completed");
    }

    public void Shutdown()
    {
        _logger.LogInformation("Shutting down DependencyLoader module");
        _loadedDependencies.Clear();
    }

    public void OnAllModulesLoaded()
    {
        _logger.LogInformation($"All modules loaded. Total dependencies loaded: {_loadedDependencies.Count}");
    }

    private void LoadAllModuleDependencies()
    {
        if (string.IsNullOrEmpty(_modulesPath))
            return;

        var moduleDirectories = Directory.GetDirectories(_modulesPath);
        
        foreach (var moduleDir in moduleDirectories)
        {
            var dependenciesPath = Path.Combine(moduleDir, "dependencies");
            
            if (Directory.Exists(dependenciesPath))
            {
                LoadDependenciesFromDirectory(dependenciesPath, Path.GetFileName(moduleDir));
            }
        }

        if (_loadedDependencies.Count > 0)
        {
            AssemblyLoadContext.Default.Resolving += OnAssemblyResolving;
            _logger.LogInformation($"Registered assembly resolver for {_loadedDependencies.Count} dependencies");
        }
    }

    private void LoadDependenciesFromDirectory(string dependenciesPath, string moduleName)
    {
        // Load native libraries from runtimes folder
        var runtimesPath = Path.Combine(dependenciesPath, "runtimes");
        if (Directory.Exists(runtimesPath))
        {
            LoadRuntimeNativeLibraries(runtimesPath, moduleName);
        }
        
        // Load managed assemblies from dependencies root
        var dllFiles = Directory.GetFiles(dependenciesPath, "*.dll", SearchOption.TopDirectoryOnly);
        
        foreach (var dllFile in dllFiles)
        {
            LoadDependencyAssembly(dllFile, moduleName);
        }
    }

    private void LoadRuntimeNativeLibraries(string runtimesPath, string moduleName)
    {
        string rid = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win-x64" : "linux-x64";

        var platformNativePath = Path.Combine(runtimesPath, rid, "native");
        if (Directory.Exists(platformNativePath))
        {
            LoadNativeLibrariesFromPath(platformNativePath, moduleName);
        }
    }

    private void LoadNativeLibrariesFromPath(string nativePath, string moduleName)
    {
        AddNativeLibraryPath(nativePath, moduleName);
        
        var nativeFiles = Directory.GetFiles(nativePath, "*", SearchOption.TopDirectoryOnly)
            .Where(IsNativeLibraryFile)
            .ToArray();

        foreach (var nativeFile in nativeFiles)
        {
            LoadSingleNativeLibrary(nativeFile, moduleName);
        }
    }

    private void LoadSingleNativeLibrary(string nativeFilePath, string moduleName)
    {
        try
        {
            var fileName = Path.GetFileName(nativeFilePath);
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var handle = LoadLibrary(nativeFilePath);
                if (handle != IntPtr.Zero)
                {
                    _logger.LogInformation($"Preloaded native library: {fileName} for module: {moduleName}");
                }
                else
                {
                    _logger.LogDebug($"Failed to preload native library: {fileName} for module: {moduleName}");
                }
            }
            else
            {
                var handle = LoadLibraryLinux(nativeFilePath, RTLD_NOW);
                if (handle != IntPtr.Zero)
                {
                    _logger.LogInformation($"Preloaded native library: {fileName} for module: {moduleName}");
                }
                else
                {
                    _logger.LogDebug($"Failed to preload native library: {fileName} for module: {moduleName}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug($"Exception preloading native library {Path.GetFileName(nativeFilePath)}: {ex.Message}");
        }
    }

    private void AddNativeLibraryPath(string nativePath, string moduleName)
    {
        if (!_nativeLibraryPaths.Contains(nativePath))
        {
            _nativeLibraryPaths.Add(nativePath);
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
                if (!currentPath.Contains(nativePath))
                {
                    Environment.SetEnvironmentVariable("PATH", $"{nativePath};{currentPath}");
                }
            }
            else
            {
                var currentPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH") ?? "";
                if (!currentPath.Contains(nativePath))
                {
                    Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", $"{nativePath}:{currentPath}");
                }
            }
            
            _logger.LogInformation($"Added native library path: {nativePath} for module: {moduleName}");
        }
    }

    private bool IsNativeLibraryFile(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return extension == ".dll";
        }

        return extension == ".so" || filePath.EndsWith(".so", StringComparison.OrdinalIgnoreCase);
    }

    private void LoadDependencyAssembly(string dllPath, string moduleName)
    {
        try
        {
            var assemblyName = Path.GetFileNameWithoutExtension(dllPath);
            
            // Skip files in runtimes folder - they are handled separately as native libraries
            var normalizedPath = dllPath.Replace('\\', '/');
            if (normalizedPath.Contains("/runtimes/"))
            {
                _logger.LogDebug($"Skipping runtime dependency: {assemblyName} for module: {moduleName}");
                return;
            }

            // Check for version conflicts
            if (_loadedDependencies.TryGetValue(assemblyName, out var existingInfo))
            {
                var newVersion = GetAssemblyVersion(dllPath);
                
                if (newVersion > existingInfo.Version)
                {
                    _logger.LogWarning($"Version conflict detected for {assemblyName}:");
                    _logger.LogWarning($"  Existing: V{existingInfo.Version} (from {existingInfo.ModuleName})");
                    _logger.LogWarning($"  New: V{newVersion} (from {moduleName})");
                    _logger.LogWarning($"  Loading newer version V{newVersion}");
                    
                    LoadAssemblyWithInfo(dllPath, assemblyName, moduleName, newVersion);
                }
                else if (newVersion < existingInfo.Version)
                {
                    _logger.LogWarning($"Version conflict detected for {assemblyName}:");
                    _logger.LogWarning($"  Existing: V{existingInfo.Version} (from {existingInfo.ModuleName})");
                    _logger.LogWarning($"  New: V{newVersion} (from {moduleName})");
                    _logger.LogWarning($"  Keeping existing newer version V{existingInfo.Version}");
                }
                else
                {
                    _logger.LogDebug($"Same version V{newVersion} of {assemblyName} already loaded, skipping");
                }
            }
            else
            {
                var version = GetAssemblyVersion(dllPath);
                LoadAssemblyWithInfo(dllPath, assemblyName, moduleName, version);
            }
        }
        catch (BadImageFormatException)
        {
            _logger.LogDebug($"Skipping native library (Bad IL format): {Path.GetFileName(dllPath)} for module: {moduleName}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to load dependency from {dllPath} for module {moduleName}: {ex.Message}");
        }
    }

    private void LoadAssemblyWithInfo(string dllPath, string assemblyName, string moduleName, Version version)
    {
        var loader = PluginLoader.CreateFromAssemblyFile(dllPath, config =>
        {
            config.PreferSharedTypes = true;
            config.IsLazyLoaded = false;
            config.IsUnloadable = false;
            config.LoadInMemory = true;
            config.EnableHotReload = false;
        });

        var assembly = loader.LoadDefaultAssembly();
        
        _loadedDependencies[assemblyName] = new AssemblyInfo
        {
            Assembly = assembly,
            Version = version,
            ModuleName = moduleName,
            FullPath = dllPath
        };

        _logger.LogInformation($"Loaded dependency: {assemblyName} V{version} for module: {moduleName}");
    }

    private Version GetAssemblyVersion(string dllPath)
    {
        try
        {
            var assemblyName = AssemblyName.GetAssemblyName(dllPath);
            return assemblyName.Version ?? new Version(0, 0, 0, 0);
        }
        catch
        {
            return new Version(0, 0, 0, 0);
        }
    }

    private Assembly? OnAssemblyResolving(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        if (assemblyName.Name != null && _loadedDependencies.TryGetValue(assemblyName.Name, out var assemblyInfo))
        {
            if (assemblyName.Version != null)
            {
                if (assemblyInfo.Version < assemblyName.Version)
                {
                    _logger.LogWarning($"Assembly {assemblyName.Name} V{assemblyName.Version} requested, but only V{assemblyInfo.Version} is available");
                }
            }
            
            return assemblyInfo.Assembly;
        }

        return null;
    }

    private class AssemblyInfo
    {
        public Assembly Assembly { get; set; } = null!;
        public Version Version { get; set; } = null!;
        public string ModuleName { get; set; } = null!;
        public string FullPath { get; set; } = null!;
    }
}
