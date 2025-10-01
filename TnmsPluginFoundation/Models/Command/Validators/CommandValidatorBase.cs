using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace TnmsPluginFoundation.Models.Command.Validators;

/// <summary>
/// Base class for command validators that provides default implementation for backward compatibility
/// </summary>
public abstract class CommandValidatorBase : ICommandValidator
{
    /// <summary>
    /// Name of this validator for identification purposes
    /// </summary>
    public abstract string ValidatorName { get; }

    /// <summary>
    /// Message of validation failure
    /// </summary>
    public abstract string ValidationFailureMessage { get; }

    /// <summary>
    /// Validates player command input
    /// </summary>
    /// <param name="player">CCSPlayerController</param>
    /// <param name="commandInfo">CommandInfo</param>
    /// <returns>TnmsCommandValidationResult</returns>
    public abstract TnmsCommandValidationResult Validate(IGameClient? player, StringCommand commandInfo);

    /// <summary>
    /// Validates player command input and returns validated arguments
    /// Default implementation calls Validate and returns empty ValidatedArguments on success
    /// Override this method to populate validated arguments
    /// </summary>
    /// <param name="player">CCSPlayerController</param>
    /// <param name="commandInfo">CommandInfo</param>
    /// <returns>TnmsCommandValidationContext</returns>
    public virtual TnmsCommandValidationContext ValidateWithArguments(IGameClient? player, StringCommand commandInfo)
    {
        var result = Validate(player, commandInfo);
        
        if (result == TnmsCommandValidationResult.Success)
        {
            var validatedArguments = ExtractArguments(player, commandInfo);
            return TnmsCommandValidationContext.Success(validatedArguments);
        }
        
        return result switch
        {
            TnmsCommandValidationResult.Failed => TnmsCommandValidationContext.Failed(),
            TnmsCommandValidationResult.FailedIgnoreDefault => TnmsCommandValidationContext.FailedIgnoreDefault(),
            _ => TnmsCommandValidationContext.Failed()
        };
    }

    /// <summary>
    /// Extracts and validates arguments from command info
    /// Override this method to populate ValidatedArguments with parsed/converted arguments
    /// </summary>
    /// <param name="player">CCSPlayerController</param>
    /// <param name="commandInfo">CommandInfo</param>
    /// <returns>ValidatedArguments containing parsed arguments</returns>
    protected virtual ValidatedArguments ExtractArguments(IGameClient? player, StringCommand commandInfo)
    {
        return new ValidatedArguments();
    }
}