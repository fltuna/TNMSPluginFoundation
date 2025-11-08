using TnmsPluginFoundation.Example.Modules.PermissionTest.Commands;
using TnmsPluginFoundation.Models.Plugin;

namespace TnmsPluginFoundation.Example.Modules.PermissionTest;

public class TnmsPermissionTest(IServiceProvider serviceProvider, bool hotReload) : PluginModuleBase(serviceProvider, hotReload)
{
    public override string PluginModuleName => "TnmsPermissionTest";
    public override string ModuleChatPrefix => "";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        RegisterTnmsCommand<AddPermission>();
        RegisterTnmsCommand<PermissionCheck>();
        RegisterTnmsCommand<RemovePermission>();
        RegisterTnmsCommand<VariablePermissionCheck>();
    }
}