using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using TnmsPluginFoundation.Models.Command.Validators.RangedValidators;

namespace TnmsPluginFoundation.Models.Command.Validators;

/// <summary>
/// Context information for validation failures
/// </summary>
public class ValidationFailureContext
{
    /// <summary>
    /// The validator that failed
    /// </summary>
    public ICommandValidator Validator { get; }
    
    /// <summary>
    /// The player who executed the command
    /// </summary>
    public IGameClient? Player { get; }
    
    /// <summary>
    /// Command information
    /// </summary>
    public StringCommand CommandInfo { get; }
    
    /// <summary>
    /// Basic validation result
    /// </summary>
    public TnmsCommandValidationResult ValidationResult { get; }
    
    /// <summary>
    /// Ranged validation result (if applicable)
    /// </summary>
    public TnmsRangedCommandValidationResult? RangedResult { get; }
    
    /// <summary>
    /// The ranged validator instance (if applicable)
    /// </summary>
    public IRangedArgumentValidator? RangedValidator { get; }

    /// <summary>
    /// Initializes a new ValidationFailureContext
    /// </summary>
    /// <param name="validator">The validator that failed</param>
    /// <param name="player">The player who executed the command</param>
    /// <param name="commandInfo">Command information</param>
    /// <param name="validationResult">Basic validation result</param>
    public ValidationFailureContext(ICommandValidator validator, IGameClient? player, StringCommand commandInfo, TnmsCommandValidationResult validationResult)
    {
        Validator = validator;
        Player = player;
        CommandInfo = commandInfo;
        ValidationResult = validationResult;
        
        // Extract ranged validator information if available
        if (validator is CompositeValidator composite)
        {
            RangedValidator = composite.GetRangedValidator();
            RangedResult = RangedValidator?.GetLastRangedResult();
        }
        else if (validator is IRangedArgumentValidator rangedValidator)
        {
            RangedValidator = rangedValidator;
            RangedResult = rangedValidator.GetLastRangedResult();
        }
    }
}