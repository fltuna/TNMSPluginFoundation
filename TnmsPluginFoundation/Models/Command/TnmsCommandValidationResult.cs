namespace TnmsPluginFoundation.Models.Command;

/// <summary>
/// Result set for TnmsPluginFoundation CommandBase
/// </summary>
public enum TnmsCommandValidationResult
{
    /// <summary>
    /// Executes command normally
    /// </summary>
    Success,
    /// <summary>
    /// Ignores command with base failure message provided by TnmsAbstractCommandBase
    /// </summary>
    Failed,
    /// <summary>
    /// Ignores command and do not print base failure message provided by TnmsAbstractCommandBase
    /// </summary>
    FailedIgnoreDefault,
}