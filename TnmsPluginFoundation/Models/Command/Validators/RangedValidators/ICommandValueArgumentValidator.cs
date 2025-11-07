namespace TnmsPluginFoundation.Models.Command.Validators.RangedValidators;

/// <summary>
/// Base interface for command validators
/// </summary>
public interface ICommandValueArgumentValidator<T>: ICommandArgumentValidator
{
    /// <summary>
    /// Returns maximum bounds for this validator
    /// </summary>
    /// <returns></returns>
    public T? GetMax();
    
    /// <summary>
    /// Returns minimum bounds for this validator
    /// </summary>
    /// <returns></returns>
    public T? GetMin();
}