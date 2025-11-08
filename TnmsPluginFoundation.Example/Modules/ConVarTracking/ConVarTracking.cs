using TnmsPluginFoundation.Models.Plugin;

namespace TnmsPluginFoundation.Example.Modules.ConVarTracking;

public class ConVarTracking(IServiceProvider serviceProvider, bool hotReload) : PluginModuleBase(serviceProvider, hotReload)
{
    public override string PluginModuleName => "ConVarTracking";
    public override string ModuleChatPrefix => "[ConVarTracking}";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        CreateAndTrackConVar("ms_test_cvar", "0", "Test ConVar");
        SharedSystem.GetModSharp().LogMessage("HotReload: " + HotReload);
    }

    protected override void OnAllModulesLoaded()
    {
    }

    protected override void OnUnloadModule()
    {
    }
}