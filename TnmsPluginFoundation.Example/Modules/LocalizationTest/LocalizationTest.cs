using TnmsPluginFoundation.Example.Modules.TestCommand.Command;
using TnmsPluginFoundation.Models.Plugin;

namespace TnmsPluginFoundation.Example.Modules.LocalizationTest;

public class LocalizationTest(IServiceProvider serviceProvider, bool hotReload) : PluginModuleBase(serviceProvider, hotReload)
{
    public override string PluginModuleName => "TestModule";
    public override string ModuleChatPrefix => "[TEST}";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        RegisterTnmsCommand<LocalizationTestCommand>();
    }

    protected override void OnAllModulesLoaded()
    {
    }

    protected override void OnUnloadModule()
    {
    }
}