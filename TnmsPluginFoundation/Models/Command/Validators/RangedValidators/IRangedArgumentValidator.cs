using System;
using System.Numerics;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace TnmsPluginFoundation.Models.Command.Validators.RangedValidators;

/// <summary>
/// Base interface for ranged command validators (non-generic)
/// </summary>
public interface IRangedArgumentValidator : ICommandArgumentValidator
{
    /// <summary>
    /// Validates range-specific command input
    /// </summary>
    /// <param name="player">CCSPlayerController</param>
    /// <param name="commandInfo">CommandInfo</param>
    /// <returns>TnmsRangedCommandValidationResult</returns>
    TnmsRangedCommandValidationResult ValidateRange(IGameClient? player, StringCommand commandInfo);
        
    /// <summary>
    /// Whether to notify when validation fails
    /// </summary>
    bool ShouldNotifyOnFailure { get; }
        
    /// <summary>
    /// Gets the last range validation result
    /// </summary>
    /// <returns>TnmsRangedCommandValidationResult</returns>
    TnmsRangedCommandValidationResult GetLastRangedResult();
        
    /// <summary>
    /// Gets description of the valid range
    /// </summary>
    /// <returns>String representation of the range</returns>
    string GetRangeDescription();
        
    /// <summary>
    /// Gets the parsed value as an object
    /// </summary>
    /// <returns>Parsed value as object or null</returns>
    object? GetParsedValueAsObject();
}

/// <summary>
/// Specialized interface for ranged command validators
/// </summary>
public interface IRangedArgumentValidator<T> : IRangedArgumentValidator, ICommandValueArgumentValidator<T?>
    where T : struct, INumber<T>, IComparable<T>
{
    /// <summary>
    /// Gets the parsed value converted to the specified type
    /// </summary>
    /// <typeparam name="TResult">Target type</typeparam>
    /// <returns>Converted value or null</returns>
    TResult? GetParsedValueAs<TResult>() where TResult : struct;
}