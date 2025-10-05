namespace TnmsAdministrationPlatform.Shared;

public enum PermissionSaveResult
{
    Success,
    Failure,
    FailureClientAlreadyInGroup,
    FailureClientDontHaveGroup,
    FailureDuplicatePermission,
    FailureDontHavePermission,
    FailureNoDatabaseConnection,
    GroupNotFound,
}