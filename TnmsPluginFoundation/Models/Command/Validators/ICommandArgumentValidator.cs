using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace TnmsPluginFoundation.Models.Command.Validators;

/// <summary>
/// Base interface for command validators
/// </summary>
public interface ICommandArgumentValidator: ICommandValidator
{
    /// <summary>
    /// Argument index of current command validator
    /// </summary>
    int ArgumentIndex { get; }
}