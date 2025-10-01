namespace TnmsPluginFoundation.Interfaces;

/// <summary>
/// Required features that recognized as a PluginModule
/// </summary>
public interface IPluginModule
{
    /// <summary>
    /// Module name should be unique
    /// </summary>
    string PluginModuleName { get; }

    /// <summary>
    /// This method called when module is registering in main plugin class using <see cref="TnmsPluginBase.RegisterModule{T}()"/>
    /// </summary>
    public void Initialize();
    
    
    /// <summary>
    /// This method called after when all plugins loaded in main plugin class using <see cref="TnmsPluginBase.CallModulesAllPluginsLoaded()"/>
    /// </summary>
    public void AllPluginsLoaded();
    
    /// <summary>
    /// This method called when plugins unloading in main plugin class using <see cref="TnmsPluginBase.UnloadAllModules()"/>
    /// </summary>
    public void UnloadModule();
}