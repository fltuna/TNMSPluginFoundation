using System;
using System.Collections.Generic;
using System.Linq;
using Sharp.Shared.Objects;
using TnmsAdministrationPlatform.Shared;
using TnmsPluginFoundation.Interfaces;

namespace TnmsPluginFoundation.Models.Admin;

/// <summary>
/// Default wrapper implementation that wraps TnmsAdministrationPlatform's IAdminManager. <br/>
/// This is used when no custom admin manager is provided via <see cref="TnmsPlugin.CreateAdminManager"/>.
/// </summary>
public sealed class TnmsAdminManagerWrapper : ITnmsPlfdAdminManager
{
    private readonly IAdminManager _adminManager;

    public TnmsAdminManagerWrapper(IAdminManager adminManager)
    {
        _adminManager = adminManager ?? throw new ArgumentNullException(nameof(adminManager));
    }

    // ========== Permission Resolving ==========

    /// <inheritdoc />
    public bool ClientHasPermission(IGameClient? client, string permissionNode)
    {
        return _adminManager.ClientHasPermission(client, permissionNode);
    }

    /// <inheritdoc />
    public bool ClientCanTarget(IGameClient client, IGameClient target)
    {
        return _adminManager.ClientCanTarget(client, target);
    }

    // ========== Admin Information ==========

    /// <inheritdoc />
    public ITnmsPlfdAdminUser? GetAdminInformation(IGameClient client)
    {
        var adminUser = _adminManager.GetAdminInformation(client);
        return adminUser != null ? new TnmsPlfdAdminUserAdapter(adminUser) : null;
    }

    // ========== Immunity ==========

    /// <inheritdoc />
    public byte GetClientImmunity(IGameClient client)
    {
        return _adminManager.GetClientImmunity(client);
    }

    /// <inheritdoc />
    public PermissionModifyResult SetClientImmunity(IGameClient client, byte immunity)
    {
        return ConvertResult(_adminManager.SetClientImmunity(client, immunity));
    }

    // ========== Permission Commands ==========

    /// <inheritdoc />
    public PermissionModifyResult AddPermissionToClient(IGameClient client, string permission)
    {
        return ConvertResult(_adminManager.AddPermissionToClient(client, permission));
    }

    /// <inheritdoc />
    public PermissionModifyResult RemovePermissionFromClient(IGameClient client, string permission)
    {
        return ConvertResult(_adminManager.RemovePermissionFromClient(client, permission));
    }

    // ========== Group Management ==========

    /// <inheritdoc />
    /// <remarks>
    /// Not supported in TnmsAdministrationPlatform v0.0.2. <br/>
    /// This will be implemented when the dependency is updated.
    /// </remarks>
    public IReadOnlyDictionary<string, ITnmsPlfdAdminGroup> GetGroups()
    {
        // TODO: Implement when TnmsAdministrationPlatform is updated with GetGroups() support
        throw new NotSupportedException(
            "GetGroups is not supported in the current version of TnmsAdministrationPlatform. " +
            "Please update the dependency or provide a custom ITnmsPlfdAdminManager implementation.");
    }

    /// <inheritdoc />
    public PermissionModifyResult AddClientToGroup(IGameClient client, string groupName)
    {
        return ConvertResult(_adminManager.AddClientToGroup(client, groupName));
    }

    /// <inheritdoc />
    public PermissionModifyResult RemoveClientFromGroup(IGameClient client, string groupName)
    {
        return ConvertResult(_adminManager.RemoveClientFromGroup(client, groupName));
    }

    /// <inheritdoc />
    public PermissionModifyResult AddPermissionToGroup(string groupName, string permission)
    {
        return ConvertResult(_adminManager.AddPermissionToGroup(groupName, permission));
    }

    /// <inheritdoc />
    public PermissionModifyResult RemovePermissionFromGroup(string groupName, string permission)
    {
        return ConvertResult(_adminManager.RemovePermissionFromGroup(groupName, permission));
    }

    // ========== Helper ==========

    private static PermissionModifyResult ConvertResult(PermissionSaveResult result)
    {
        return result switch
        {
            PermissionSaveResult.Success => PermissionModifyResult.Success,
            PermissionSaveResult.FailureClientAlreadyInGroup => PermissionModifyResult.ClientAlreadyInGroup,
            PermissionSaveResult.FailureClientDontHaveGroup => PermissionModifyResult.ClientNotInGroup,
            PermissionSaveResult.FailureDuplicatePermission => PermissionModifyResult.DuplicatePermission,
            PermissionSaveResult.FailureDontHavePermission => PermissionModifyResult.PermissionNotFound,
            PermissionSaveResult.FailureNoDatabaseConnection => PermissionModifyResult.NoDatabaseConnection,
            PermissionSaveResult.GroupNotFound => PermissionModifyResult.GroupNotFound,
            _ => PermissionModifyResult.Failure,
        };
    }
}
