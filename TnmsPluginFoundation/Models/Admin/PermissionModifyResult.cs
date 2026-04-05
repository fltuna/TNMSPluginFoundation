namespace TnmsPluginFoundation.Models.Admin;

/// <summary>
/// Result of permission modification operations.
/// </summary>
public enum PermissionModifyResult
{
    Success,
    Failure,
    ClientAlreadyInGroup,
    ClientNotInGroup,
    DuplicatePermission,
    PermissionNotFound,
    NoDatabaseConnection,
    GroupNotFound,
}
