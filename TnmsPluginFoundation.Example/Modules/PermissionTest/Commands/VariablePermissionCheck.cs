using Microsoft.Extensions.Logging;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using TnmsPluginFoundation.Extensions.Client;
using TnmsPluginFoundation.Models.Command;
using TnmsPluginFoundation.Models.Command.Validators;

namespace TnmsPluginFoundation.Example.Modules.PermissionTest.Commands;

public class VariablePermissionCheck(IServiceProvider provider) : TnmsAbstractCommandBase(provider)
{
    // ms_ is automatically declared so we don't need to specify here
    public override string CommandName => "vpc";
    public override string CommandDescription => "";
    public override TnmsCommandRegistrationType CommandRegistrationType => TnmsCommandRegistrationType.Client;

    protected override ICommandValidator? GetValidator() => new ArgumentCountValidator(1, true);

    protected override ValidationFailureResult OnValidationFailed(ValidationFailureContext context)
    {
        Logger.LogInformation("Validation failed");
        return base.OnValidationFailed(context);
    }

    protected override void ExecuteCommand(IGameClient? player, StringCommand commandInfo, ValidatedArguments? validatedArguments)
    {
        if (player == null)
            return;
        
        if (TnmsPlugin.AdminManager.ClientHasPermission(player, commandInfo.GetArg(1)))
        {
            player.PrintToChat($"You have permission: {commandInfo.GetArg(1)}");
        }
        else
        {
            player.PrintToChat($"You don't have permission: {commandInfo.GetArg(1)}");
        }
    }
}