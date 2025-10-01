using Microsoft.Extensions.Logging;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using TnmsPluginFoundation.Models.Command;

namespace TnmsPluginFoundation.Example.Modules.TestCommand.Command;

public class FirstTestCommand(IServiceProvider provider) : TnmsAbstractCommandBase(provider)
{
    public override string CommandName => "ms_first";
    public override string CommandDescription => "This is the first test command.";
    
    protected override void ExecuteCommand(IGameClient? player, StringCommand commandInfo, ValidatedArguments? validatedArguments)
    {
        Plugin.Logger.LogInformation($"cmdName: {commandInfo.CommandName}, arguments: {validatedArguments}, is chat trigger: {commandInfo.ChatTrigger}");
        
        Plugin.Logger.LogInformation("Executed");
    }
}