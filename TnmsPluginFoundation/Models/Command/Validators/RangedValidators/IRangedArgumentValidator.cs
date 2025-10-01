﻿using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace TnmsPluginFoundation.Models.Command.Validators.RangedValidators;

/// <summary>
/// Specialized interface for ranged command validators
/// </summary>
public interface IRangedArgumentValidator
{
    /// <summary>
    /// Name of this validator for identification purposes
    /// </summary>
    string ValidatorName { get; }

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
        
    /// <summary>
    /// Gets the parsed value converted to the specified type
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    /// <returns>Converted value or null</returns>
    T? GetParsedValueAs<T>() where T : struct;
}