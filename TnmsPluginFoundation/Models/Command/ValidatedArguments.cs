using System;
using System.Collections.Generic;
using Sharp.Shared.Objects;
using TnmsExtendableTargeting.Shared;

namespace TnmsPluginFoundation.Models.Command;

/// <summary>
/// Contains validated command arguments that have been processed by validators
/// </summary>
public class ValidatedArguments
{
    private readonly Dictionary<int, object?> _indexedArguments = new();

    /// <summary>
    /// Gets a validated argument by index
    /// </summary>
    /// <typeparam name="T">Expected type of the argument</typeparam>
    /// <param name="index">Index of the argument (0-based)</param>
    /// <returns>The validated argument value</returns>
    public T? GetArgument<T>(int index)
    {
        if (_indexedArguments.TryGetValue(index, out var value))
        {
            if (value is T typedValue)
                return typedValue;
            
            // ITargetingResult → List<IGameClient> conversion for backwards compatibility for ExtendableTargetValidator
            if (typeof(T) == typeof(List<IGameClient>) && value is ITargetingResult result)
            {
                return (T)(object)result.GetTargets();
            }
            
            if (value != null && typeof(T) != typeof(object))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    // ignored
                }
            }
        }
        
        return default;
    }

    /// <summary>
    /// Sets a validated argument by index
    /// </summary>
    /// <param name="index">Index of the argument (0-based)</param>
    /// <param name="value">Value of the argument</param>
    internal void SetArgument(int index, object? value)
    {
        _indexedArguments[index] = value;
    }

    /// <summary>
    /// Checks if an argument exists by index
    /// </summary>
    /// <param name="index">Index of the argument</param>
    /// <returns>True if the argument exists</returns>
    public bool HasArgument(int index)
    {
        return _indexedArguments.ContainsKey(index);
    }

    /// <summary>
    /// Gets all argument indices
    /// </summary>
    /// <returns>Collection of argument indices</returns>
    public IEnumerable<int> GetArgumentIndices()
    {
        return _indexedArguments.Keys;
    }

    /// <summary>
    /// Gets the count of indexed arguments
    /// </summary>
    public int ArgumentCount => _indexedArguments.Count;
}