﻿using TnmsPluginFoundation.Example.Modules.TargetingTest.Commands;
using TnmsPluginFoundation.Models.Plugin;

namespace TnmsPluginFoundation.Example.Modules.TargetingTest;

public class TnmsTargetingTest(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "TnmsPermissionTest";
    public override string ModuleChatPrefix => "";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        RegisterTnmsCommand<TargetTestCommand>();
    }
}