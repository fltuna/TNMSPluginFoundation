using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace TnmsPluginFoundation.Models.Command.Validators;

/// <summary>
/// Base interface for command validators
/// </summary>
public interface ICommandValidator
{
    /// <summary>
    /// Name of this validator for identification purposes
    /// </summary>
    string ValidatorName { get; }

    /// <summary>
    /// Message of validation failure
    /// </summary>
    string ValidationFailureMessage { get; }

    /// <summary>
    /// Validates player command input
    /// </summary>
    /// <param name="client">CCSPlayerController</param>
    /// <param name="commandInfo">CommandInfo</param>
    /// <returns>TnmsCommandValidationResult</returns>
    TnmsCommandValidationResult Validate(IGameClient? client, StringCommand commandInfo);

    /// <summary>
    /// Validates player command input and returns validated arguments
    /// </summary>
    /// <param name="player">CCSPlayerController</param>
    /// <param name="commandInfo">CommandInfo</param>
    /// <returns>TnmsCommandValidationContext</returns>
    TnmsCommandValidationContext ValidateWithArguments(IGameClient? player, StringCommand commandInfo);
}