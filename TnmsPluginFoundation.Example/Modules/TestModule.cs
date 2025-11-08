using Microsoft.Extensions.Logging;
using TnmsPluginFoundation.Models.Plugin;

namespace TnmsPluginFoundation.Example.Modules;

public class TestModule(IServiceProvider serviceProvider, bool hotReload) : PluginModuleBase(serviceProvider, hotReload)
{
    public override string PluginModuleName => "TestModule";
    public override string ModuleChatPrefix => "[TEST}";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        Plugin.Logger.LogInformation("Hello from TnmsPluginFoundation.Example.Modules.TestModule at OnInitialize()");
        Plugin.Logger.LogInformation("Is this hot reload? {hotReload}", HotReload);
    }

    protected override void OnAllModulesLoaded()
    {
        Plugin.Logger.LogInformation("Hello from TnmsPluginFoundation.Example.Modules.TestModule at OnAllPluginsLoaded()");
    }

    protected override void OnUnloadModule()
    {
        Plugin.Logger.LogInformation("Hello from TnmsPluginFoundation.Example.Modules.TestModule at OnUnloadModule()");
    }
}