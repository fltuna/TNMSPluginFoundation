using Microsoft.Extensions.Logging;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using TnmsPluginFoundation.Models.Command;

namespace TnmsPluginFoundation.Example.Modules.TestCommand.Command;

public class FirstTestCommand(IServiceProvider provider) : TnmsAbstractCommandBase(provider)
{
    // ms_ is automatically declared so we don't need to specify here
    public override string CommandName => "first";
    public override string CommandDescription => "This is the first test command.";
    
    protected override void ExecuteCommand(IGameClient? player, StringCommand commandInfo, ValidatedArguments? validatedArguments)
    {
        Plugin.Logger.LogInformation($"cmdName: {commandInfo.CommandName}, arguments: {validatedArguments}, is chat trigger: {commandInfo.ChatTrigger}");
        
        Plugin.Logger.LogInformation("Executed");
    }
}