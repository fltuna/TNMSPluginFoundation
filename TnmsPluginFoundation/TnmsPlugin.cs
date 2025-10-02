using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Managers;
using Sharp.Shared.Objects;
using TnmsAdministrationPlatform;
using TnmsExtendableTargeting.Shared;
using TnmsPluginFoundation.Interfaces;
using TnmsPluginFoundation.Models.Command;
using TnmsPluginFoundation.Models.Logger;
using TnmsPluginFoundation.Models.Plugin;

namespace TnmsPluginFoundation;

public abstract class TnmsPlugin: IModSharpModule
{
    protected TnmsPlugin(ISharedSystem  sharedSystem,
        string         dllPath,
        string         sharpPath,
        Version?       version,
        IConfiguration coreConfiguration,
        bool           hotReload)
    {
        ArgumentNullException.ThrowIfNull(dllPath);
        ArgumentNullException.ThrowIfNull(sharpPath);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(coreConfiguration);
        ArgumentNullException.ThrowIfNull(sharedSystem);
        _sharedSystem = sharedSystem;
        _hotReload = hotReload;

        var   factory = _sharedSystem.GetLoggerFactory();

        // ReSharper disable VirtualMemberCallInConstructor
        var   logger  = factory.CreateLogger(DisplayName);
        // ReSharper restore VirtualMemberCallInConstructor

        Logger = logger;
        GameData = _sharedSystem.GetModSharp().GetGameData();
        AssetPath = Path.Combine(sharpPath, "assets");
    }
    private readonly bool _hotReload;

    /// <summary>
    /// ModSharp Shared system / Shared Modules
    /// </summary>
    internal static ISharedSystem StaticSharedSystem => _sharedSystem;

    public static IExtendableTargeting ExtendableTargeting => _extendableTargeting;
    private static IExtendableTargeting _extendableTargeting = null!;

    public static IAdminManager AdminManager => _adminManager;
    private static IAdminManager _adminManager = null!;
    
    

    /*
     * Mod Sharp Related
     */
    public ISharedSystem SharedSystem => _sharedSystem;
    private static ISharedSystem _sharedSystem = null!;

    public IConVarManager ConVarManager => _sharedSystem.GetConVarManager();
    
    
    public abstract string DisplayName { get; }
    public abstract string DisplayAuthor { get; }
    
    
    /// <summary>
    /// ConVarConfigurationService for managing ConVar config.
    /// </summary>
    public ConVarConfigurationService ConVarConfigurationService { get; private set; } = null!;
    
    /// <summary>
    /// DebugLogger instance
    /// </summary>
    public IDebugLogger? DebugLogger { get; protected set; }

    /// <summary>
    /// Plugin Logger
    /// </summary>
    public ILogger Logger { get; }
    
    /// <summary>
    /// ModSharp shared gamedata system
    /// </summary>
    public IGameData GameData { get; }
    
    /// <summary>
    /// Asset path
    /// </summary>
    public string AssetPath { get; }
    
    /// <summary>
    /// Base config directory path you can use whatever.
    /// </summary>
    public abstract string BaseCfgDirectoryPath { get; }
    
    /// <summary>
    /// ConVar configuration path, this path is used for saving All ConVar config.
    /// Relative path from game/csgo/cfg/
    /// Also if this path defined a specific file, then module config is not generated.
    /// </summary>
    public abstract string ConVarConfigPath { get; }

    private ServiceCollection ServiceCollection { get; } = new();
    
    /// <summary>
    /// DI Container
    /// </summary>
    private ServiceProvider ServiceProvider { get; set; } = null!;
    
    /// <summary>
    /// This prefix used for printing to chat.
    /// </summary>
    public abstract string PluginPrefix { get; }
    
    /// <summary>
    /// Is PluginPrefix is translation key?
    /// </summary>
    public abstract bool UseTranslationKeyInPluginPrefix { get; }


    /// <summary>
    /// We can register required services that use entire plugin or modules.
    /// At this time, we can get ConVarConfigurationService and AbstractTnmsPluginBase instance from DI container in this method.
    /// </summary>
    protected virtual void RegisterRequiredPluginServices(IServiceCollection collection ,IServiceProvider provider){}
    
    /// <summary>
    /// You can register any services. for instance: external feature obtained from ModSharp's ModuleManager.<br/>
    /// This is a final chance of registering services to DI container.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="provider"></param>
    protected virtual void LateRegisterPluginServices(IServiceCollection collection, IServiceProvider provider){}

    private void UpdateServices()
    {
        foreach (PluginModuleBase module in _loadedModules)
        {
            module.UpdateServices(ServiceProvider);
        }
    }

    /// <summary>
    /// ModShaprp's module initialization
    /// </summary>
    /// <returns></returns>
    public bool Init()
    {
        ConVarConfigurationService = new(this);
        // Add self and core service to DI Container
        ServiceCollection.AddSingleton(this);
        ServiceCollection.AddSingleton(ConVarConfigurationService);
        
        // Build first ServiceProvider, because we need a plugin instance to initialize modules
        RebuildServiceProvider();
        
        // Then call register required plugin services
        RegisterRequiredPluginServices(ServiceCollection, ServiceProvider);

        DebugLogger ??= new IgnoredLogger();

        RegisterDebugLogger(DebugLogger);
        
        // And build again
        RebuildServiceProvider();
        
        // Call customizable OnLoad method
        TnmsOnPluginLoad(_hotReload);
        return true;
    }

    /// <summary>
    /// This method is can be used to initialize plugin feature.
    /// </summary>
    /// <param name="hotReload">Is hot reload?</param>
    protected virtual void TnmsOnPluginLoad(bool hotReload){}


    /// <summary>
    /// ModSharp's module initialization after all plugins loaded
    /// </summary>
    public void PostInit()
    {
        TnmsLateOnPluginLoad(ServiceProvider);
    }

    /// <summary>
    /// You can register custom module here.
    /// All modules registered DI dependency is accessible in here.
    /// </summary>
    protected virtual void TnmsLateOnPluginLoad(ServiceProvider provider){}



    /// <summary>
    /// ModSharp's module initialization after all plugins loaded.
    /// </summary>
    /// <returns></returns>
    public void OnAllModulesLoaded()
    {
        LateRegisterPluginServices(ServiceCollection, ServiceProvider);
        RebuildServiceProvider();
        UpdateServices();
        
        TnmsAllPluginsLoaded(_hotReload);
        CallModulesAllPluginsLoaded();
        ConVarConfigurationService.SaveAllConfigToFile();
        ConVarConfigurationService.ExecuteConfigs();

        var adminSystem = _sharedSystem.GetSharpModuleManager().GetRequiredSharpModuleInterface<IAdminManager>(IAdminManager.ModSharpModuleIdentity).Instance;
        _adminManager = adminSystem ?? throw new InvalidOperationException("TnmsAdministrationPlatform is not found! Make sure TnmsAdministrationPlatform is installed!");

        var extendableTargeting = _sharedSystem.GetSharpModuleManager()
            .GetRequiredSharpModuleInterface<IExtendableTargeting>(IExtendableTargeting.ModSharpModuleIdentity).Instance;
        _extendableTargeting = extendableTargeting ?? throw new InvalidOperationException("TnmsExtendableTargeting is not found! Make sure TnmsExtendableTargeting is installed!");
    }

    /// <summary>
    /// This method is can be used to late initialize plugin feature.
    /// </summary>
    /// <param name="hotReload">Is hot reload?</param>
    protected virtual void TnmsAllPluginsLoaded(bool hotReload){}


    /// <summary>
    /// ModSharp's module shutdown.
    /// </summary>
    /// <returns></returns>
    public void Shutdown()
    {
        TnmsOnPluginUnload(_hotReload);
        UnloadAllModules();
        
        // Use reverse iteration to avoid collection modification issues
        for (int i = TnmsAbstractedCommands.Count - 1; i >= 0; i--)
        {
            RemoveTnmsCommand(TnmsAbstractedCommands[i]);
        }
        TnmsAbstractedCommands.Clear();
    }

    /// <summary>
    /// This method is can be used to unload plugin feature.
    /// </summary>
    /// <param name="hotReload">Is hot reload?</param>
    protected virtual void TnmsOnPluginUnload(bool hotReload){}


    private void RebuildServiceProvider()
    {
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }
    
    /// <summary>
    /// Localize string with plugin prefix.
    /// </summary>
    /// <param name="languageKey">Language Key</param>
    /// <param name="args">Any args that can be use ToString()</param>
    /// <returns>"{PluginPrefix} {LocalizedString}"</returns>
    [Obsolete("This method is deprecated, please use PluginModuleBase::LocalizeWithPluginPrefix()", true)]
    public string LocalizeStringWithPluginPrefix(string languageKey, params object[] args)
    {
        return $"{PluginPrefix} {LocalizeString(languageKey, args)}";
    }

    /// <summary>
    /// Same as Plugin.Localizer[langaugeKey, args]
    /// </summary>
    /// <param name="localizationKey">Localization Key</param>
    /// <param name="args">Any args that can be use ToString()</param>
    /// <returns>Translated result</returns>
    public string LocalizeString(string localizationKey, params object[] args)
    {
        // TODO() Implement Localizer
        return "";
    }
    
    /// <summary>
    /// Same as Plugin.Localizer.ForPlayer(player, localizationKey, args)
    /// </summary>
    /// <param name="client">Player instance</param>
    /// <param name="localizationKey">Localization Key</param>
    /// <param name="args">Any args that can be use ToString()</param>
    /// <returns>Translated result as player's language</returns>
    public string LocalizeStringForPlayer(IGameClient client, string localizationKey, params object[] args)
    {
        // TODO() Implement Player Based Localizer
        return "";
    }
    

    private readonly HashSet<PluginModuleBase> _loadedModules = [];

    /// <summary>
    /// Register module without hot reload state.
    /// </summary>
    /// <typeparam name="T">Any classes that inherited a PluginModuleBase</typeparam>
    protected void RegisterModule<T>() where T : PluginModuleBase
    {
        var module = (T)Activator.CreateInstance(typeof(T), ServiceProvider)!;
        _loadedModules.Add(module);
        module.Initialize();
        module.RegisterServices(ServiceCollection);
        // Rebuild, because some modules are depend on other modules.
        // And if the module is API or something, required before call OnAllPluginsLoaded.
        RebuildServiceProvider();
        Logger.LogInformation($"{module.PluginModuleName} has been initialized");
    }

    /// <summary>
    /// Register module with hot reload state.
    /// </summary>
    /// <typeparam name="T">Any classes that inherited a PluginModuleBase</typeparam>
    protected void RegisterModule<T>(bool hotReload) where T : PluginModuleBase
    {
        var module = (T)Activator.CreateInstance(typeof(T), ServiceProvider, hotReload)!;
        _loadedModules.Add(module);
        module.Initialize();
        module.RegisterServices(ServiceCollection);
        // Rebuild, because some modules are depend on other modules.
        // And if the module is API or something, required before call OnAllPluginsLoaded.
        RebuildServiceProvider();
        Logger.LogInformation($"{module.PluginModuleName} has been initialized");
    }

    private void CallModulesAllPluginsLoaded()
    {
        foreach (IPluginModule loadedModule in _loadedModules)
        {
            loadedModule.AllPluginsLoaded();
        }
    }


    private void UnloadAllModules()
    {
        foreach (PluginModuleBase loadedModule in _loadedModules)
        {
            loadedModule.UnloadModule();
            Logger.LogInformation($"{loadedModule.PluginModuleName} has been unloaded.");
        }
        _loadedModules.Clear();
    }

    private void RegisterDebugLogger(IDebugLogger logger)
    {
        ServiceCollection.AddSingleton(logger);
    }
    
    
    private List<TnmsAbstractCommandBase> TnmsAbstractedCommands { get; } = new();

    /// <summary>
    /// Add TnmsAbstracted command to ModSharp
    /// </summary>
    /// <param name="command">Classes that inherited TnmsAbstractCommandBase</param>
    /// <param name="registrationType">Command regstration type</param>
    public void AddTnmsCommand(TnmsAbstractCommandBase command)
    {
        if (command.CommandRegistrationType == 0)
            throw new ArgumentException("Command registration type should have at least 1 flag!");
        
        if (command.CommandRegistrationType.HasFlag(TnmsCommandRegistrationType.Client))
            SharedSystem.GetClientManager().InstallCommandCallback(command.CommandName, command.Execute);
        
        if (command.CommandRegistrationType.HasFlag(TnmsCommandRegistrationType.Server))
            SharedSystem.GetConVarManager().CreateConsoleCommand("ms_" + command.CommandName, command.Execute, command.CommandDescription, command.ConVarFlags);

        TnmsAbstractedCommands.Add(command);
    }

    /// <summary>
    /// Remove TnmsAbstracted command to ModSharp
    /// </summary>
    /// <param name="command">Classes that inherited TnmsAbstractCommandBase</param>
    /// <param name="registrationType">Command regstration type</param>
    public void RemoveTnmsCommand(TnmsAbstractCommandBase command)
    {
        if (command.CommandRegistrationType == 0)
            throw new ArgumentException("Command registration type should have at least 1 flag!");
        
        if (command.CommandRegistrationType.HasFlag(TnmsCommandRegistrationType.Client))
            SharedSystem.GetClientManager().RemoveCommandCallback(command.CommandName, command.Execute);
        
        if (command.CommandRegistrationType.HasFlag(TnmsCommandRegistrationType.Server))
            SharedSystem.GetConVarManager().ReleaseCommand("ms_" + command.CommandName);

        TnmsAbstractedCommands.Remove(command);
    }
}
