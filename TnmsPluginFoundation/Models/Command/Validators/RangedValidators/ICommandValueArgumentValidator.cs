namespace TnmsPluginFoundation.Models.Command.Validators.RangedValidators;

/// <summary>
/// Base interface for command validators
/// </summary>
public interface ICommandValueArgumentValidator<out T>: ICommandArgumentValidator
{
    /// <summary>
    /// Returns maximum bounds for this validator
    /// </summary>
    T? Max { get; }
    
    /// <summary>
    /// Returns minimum bounds for this validator
    /// </summary>
    T? Min { get; }
}