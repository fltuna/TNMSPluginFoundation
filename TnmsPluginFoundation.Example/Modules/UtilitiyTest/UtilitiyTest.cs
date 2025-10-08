using TnmsPluginFoundation.Example.Modules.LocalizationTest;
using TnmsPluginFoundation.Models.Plugin;

namespace TnmsPluginFoundation.Example.Modules.UtilitiyTest;

public class UtilitiyTest(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "UtilitiyTest";
    public override string ModuleChatPrefix => "[TEST}";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        RegisterTnmsCommand<UtilitiyTestCommand>();
    }

    protected override void OnAllModulesLoaded()
    {
    }

    protected override void OnUnloadModule()
    {
    }
}