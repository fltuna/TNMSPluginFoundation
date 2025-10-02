namespace TnmsPluginFoundation.Models.Command;

/// <summary>
/// Contains the result of command validation along with validated arguments
/// </summary>
public class TnmsCommandValidationContext(
    TnmsCommandValidationResult result,
    ValidatedArguments? validatedArguments = null)
{
    /// <summary>
    /// The validation result
    /// </summary>
    public TnmsCommandValidationResult Result { get; } = result;

    /// <summary>
    /// The validated arguments (null if validation failed)
    /// </summary>
    public ValidatedArguments? ValidatedArguments { get; } = validatedArguments;

    /// <summary>
    /// Creates a successful validation context with validated arguments
    /// </summary>
    /// <param name="validatedArguments">The validated arguments</param>
    /// <returns>Successful validation context</returns>
    public static TnmsCommandValidationContext Success(ValidatedArguments validatedArguments)
    {
        return new TnmsCommandValidationContext(TnmsCommandValidationResult.Success, validatedArguments);
    }

    /// <summary>
    /// Creates a failed validation context
    /// </summary>
    /// <returns>Failed validation context</returns>
    public static TnmsCommandValidationContext Failed()
    {
        return new TnmsCommandValidationContext(TnmsCommandValidationResult.Failed);
    }

    /// <summary>
    /// Creates a failed validation context that ignores default messages
    /// </summary>
    /// <returns>Failed validation context that ignores defaults</returns>
    public static TnmsCommandValidationContext FailedIgnoreDefault()
    {
        return new TnmsCommandValidationContext(TnmsCommandValidationResult.FailedIgnoreDefault);
    }
}