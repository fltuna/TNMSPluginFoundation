﻿using System.Reflection;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using TnmsPluginFoundation.Models.Command.Validators.RangedValidators;

namespace TnmsPluginFoundation.Models.Command.Validators;

/// <summary>
/// Composite validator that combines multiple validators
/// </summary>
public class CompositeValidator : ICommandValidator
{
    private readonly List<ICommandValidator> _validators = new();

    /// <summary>
    /// Name of this validator for identification purposes
    /// </summary>
    public virtual string ValidatorName => $"Composite[{string.Join(",", _validators.Select<ICommandValidator, string>(v => v.ValidatorName)
    )}]";

    /// <summary>
    /// Message of validation failure
    /// </summary>
    public string ValidationFailureMessage => _lastFailedValidator?.ValidationFailureMessage ?? "Common.Validation.CompositeValidator.Failure";
    
    /// <summary>
    /// Last validator that failed during validation
    /// </summary>
    private ICommandValidator? _lastFailedValidator;

    /// <summary>
    /// Gets the last validator that failed during validation
    /// </summary>
    /// <returns>The failed validator or null if no validation was performed or all passed</returns>
    public ICommandValidator? GetLastFailedValidator() => _lastFailedValidator;

    /// <summary>
    /// Adds a validator to the composite
    /// </summary>
    /// <param name="validator">Validator to add</param>
    /// <returns>This instance for method chaining</returns>
    public CompositeValidator Add(ICommandValidator validator)
    {
        _validators.Add(validator);
        return this;
    }

    /// <summary>
    /// Validates command input using all registered validators
    /// </summary>
    /// <param name="client">CCSPlayerController</param>
    /// <param name="commandInfo">CommandInfo</param>
    /// <returns>TnmsCommandValidationResult</returns>
    public TnmsCommandValidationResult Validate(IGameClient? client, StringCommand commandInfo)
    {
        foreach (var validator in _validators)
        {
            var result = validator.Validate(client, commandInfo);
            if (result != TnmsCommandValidationResult.Success)
            {
                _lastFailedValidator = validator;
                return result;
            }
        }
        return TnmsCommandValidationResult.Success;
    }

    /// <summary>
    /// Validates command input using all registered validators and returns validated arguments
    /// </summary>
    /// <param name="player">CCSPlayerController</param>
    /// <param name="commandInfo">CommandInfo</param>
    /// <returns>TnmsCommandValidationContext</returns>
    public TnmsCommandValidationContext ValidateWithArguments(IGameClient? player, StringCommand commandInfo)
    {
        var validatedArguments = new ValidatedArguments();
        
        foreach (var validator in _validators)
        {
            var validationContext = validator.ValidateWithArguments(player, commandInfo);
            
            if (validationContext.Result != TnmsCommandValidationResult.Success)
            {
                _lastFailedValidator = validator;
                return validationContext;
            }
            
            // Merge validated arguments from this validator
            if (validationContext.ValidatedArguments != null)
            {
                foreach (var index in validationContext.ValidatedArguments.GetArgumentIndices())
                {
                    var value = validationContext.ValidatedArguments.GetArgument<object>(index);
                    validatedArguments.SetArgument(index, value);
                }
            }
        }
        
        return TnmsCommandValidationContext.Success(validatedArguments);
    }

    /// <summary>
    /// Gets the first validator of the specified type
    /// </summary>
    /// <typeparam name="T">Validator type</typeparam>
    /// <returns>Validator instance or null</returns>
    public T? GetValidator<T>() where T : class, ICommandValidator
    {
        return _validators.OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Gets the first ranged validator
    /// </summary>
    /// <returns>IRangedArgumentValidator or null</returns>
    public IRangedArgumentValidator? GetRangedValidator()
    {
        return _validators.OfType<IRangedArgumentValidator>().FirstOrDefault();
    }

    /// <summary>
    /// Gets all ranged validators
    /// </summary>
    /// <returns>Enumerable of IRangedArgumentValidator</returns>
    public IEnumerable<IRangedArgumentValidator> GetAllRangedValidators()
    {
        return _validators.OfType<IRangedArgumentValidator>();
    }

    /// <summary>
    /// Gets the first ranged value converted to the specified type
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    /// <returns>Converted value or null</returns>
    public T? GetRangedValueAs<T>() where T : struct
    {
        var rangedValidator = GetRangedValidator();
        return rangedValidator?.GetParsedValueAs<T>();
    }

    /// <summary>
    /// Gets all ranged values converted to the specified type
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    /// <returns>Array of converted values</returns>
    public T?[] GetAllRangedValuesAs<T>() where T : struct
    {
        return GetAllRangedValidators()
            .Select(v => v.GetParsedValueAs<T>())
            .ToArray();
    }

    /// <summary>
    /// Gets ranged value from validator at specific argument index
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    /// <param name="argumentIndex">Argument index to find</param>
    /// <returns>Converted value or null</returns>
    public T? GetRangedValueAs<T>(int argumentIndex) where T : struct
    {
        return GetAllRangedValidators()
            .FirstOrDefault(v => HasArgumentIndex(v, argumentIndex))?.GetParsedValueAs<T>();
    }

    /// <summary>
    /// Checks if a ranged validator has the specified argument index
    /// </summary>
    /// <param name="validator">Validator to check</param>
    /// <param name="argumentIndex">Target argument index</param>
    /// <returns>True if validator has the specified argument index</returns>
    private static bool HasArgumentIndex(IRangedArgumentValidator validator, int argumentIndex)
    {
        var field = validator.GetType().GetField("_argumentIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        return field != null && (int)field.GetValue(validator)! == argumentIndex;
    }
}