using Microsoft.Extensions.Logging;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using TnmsPluginFoundation.Models.Command;

namespace TnmsPluginFoundation.Example.Modules.LocalizationTest;

public class LocalizationTestCommand(IServiceProvider provider) : TnmsAbstractCommandBase(provider)
{
    // ms_ is automatically declared so we don't need to specify here
    public override string CommandName => "l";
    public override string CommandDescription => "This is the first test command.";
    public override TnmsCommandRegistrationType CommandRegistrationType { get; } = TnmsCommandRegistrationType.Client | TnmsCommandRegistrationType.Server;

    protected override void ExecuteCommand(IGameClient? player, StringCommand commandInfo, ValidatedArguments? validatedArguments)
    {
        PrintMessageToServerOrPlayerChat(player, 
            LocalizeString(player, "Example.Modules.LocalizationTest.LocalizationTest.Message", "Format0", "Format1"));
    }
}