using Microsoft.Extensions.Logging;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using TnmsPluginFoundation.Extensions.Client;
using TnmsPluginFoundation.Models.Command;
using TnmsPluginFoundation.Models.Command.Validators;

namespace TnmsPluginFoundation.Example.Modules.PermissionTest.Commands;

public class PermissionCheck(IServiceProvider provider) : TnmsAbstractCommandBase(provider)
{
    // ms_ is automatically declared so we don't need to specify here
    public override string CommandName => "checkpermission";
    public override List<string> CommandAliases { get; } = ["checkperm", "cp"];
    public override string CommandDescription => "";
    public override TnmsCommandRegistrationType CommandRegistrationType => TnmsCommandRegistrationType.Client;

    private const string PermissionNode = "tnms.custom.permission";
    
    protected override ICommandValidator GetValidator() => new PermissionValidator(PermissionNode, true);

    protected override ValidationFailureResult OnValidationFailed(ValidationFailureContext context)
    {
        Logger.LogInformation("Validation failed by {validator}", context.Validator.ValidatorName);
        context.Client!.GetPlayerController()!.PrintToChat("Failed to validate command cuz you don't have permission.");
        return base.OnValidationFailed(context);
    }

    protected override void ExecuteCommand(IGameClient? player, StringCommand commandInfo, ValidatedArguments? validatedArguments)
    {
        Plugin.Logger.LogInformation("User has {permission}", PermissionNode);
        player!.GetPlayerController()!.PrintToChat("Validated cuz you have permission.");
    }
}