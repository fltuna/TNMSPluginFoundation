using TnmsPluginFoundation.Example.Modules.TestCommand.Command;
using TnmsPluginFoundation.Models.Plugin;

namespace TnmsPluginFoundation.Example.Modules.TestCommand;

public class TestCommand(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "TestModule";
    public override string ModuleChatPrefix => "[TEST}";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        RegisterTnmsCommand<FirstTestCommand>();
    }

    protected override void OnAllModulesLoaded()
    {
    }

    protected override void OnUnloadModule()
    {
    }
}