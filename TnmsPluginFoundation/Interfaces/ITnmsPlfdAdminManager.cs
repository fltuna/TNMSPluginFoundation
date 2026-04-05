using System.Collections.Generic;
using Sharp.Shared.Objects;
using TnmsPluginFoundation.Models.Admin;

namespace TnmsPluginFoundation.Interfaces;

/// <summary>
/// Abstraction layer for admin management system. <br/>
/// Implement this interface to provide custom admin management functionality. <br/>
/// Override <see cref="TnmsPlugin.CreateAdminManager"/> to provide custom implementation.
/// </summary>
public interface ITnmsPlfdAdminManager
{
    // ========== Permission Resolving ==========

    /// <summary>
    /// Checks if the client has the specified permission.
    /// </summary>
    /// <param name="client">Target client</param>
    /// <param name="permissionNode">Permission node to check</param>
    /// <returns>true if the client has the permission</returns>
    bool ClientHasPermission(IGameClient? client, string permissionNode);

    /// <summary>
    /// Checks if the client can target another client.
    /// </summary>
    /// <param name="client">Source client</param>
    /// <param name="target">Target client</param>
    /// <returns>true if the client can target the target</returns>
    bool ClientCanTarget(IGameClient client, IGameClient target);

    // ========== Admin Information ==========

    /// <summary>
    /// Get admin information associated with specified client.
    /// </summary>
    /// <param name="client">Client to query</param>
    /// <returns>Admin user information, or null if not found</returns>
    ITnmsPlfdAdminUser? GetAdminInformation(IGameClient client);

    // ========== Immunity ==========

    /// <summary>
    /// Get immunity level of specified client.
    /// </summary>
    /// <param name="client">Client to get immunity from</param>
    /// <returns>Immunity level (0-255)</returns>
    byte GetClientImmunity(IGameClient client);

    /// <summary>
    /// Set immunity level of specified client.
    /// </summary>
    /// <param name="client">Client to set immunity for</param>
    /// <param name="immunity">Immunity level (0-255)</param>
    /// <returns>Result of the operation</returns>
    PermissionModifyResult SetClientImmunity(IGameClient client, byte immunity);

    // ========== Permission Commands ==========

    /// <summary>
    /// Adds a permission to the specified client.
    /// </summary>
    /// <param name="client">Target client</param>
    /// <param name="permission">Permission to add</param>
    /// <returns>Result of the operation</returns>
    PermissionModifyResult AddPermissionToClient(IGameClient client, string permission);

    /// <summary>
    /// Removes a permission from the specified client.
    /// </summary>
    /// <param name="client">Target client</param>
    /// <param name="permission">Permission to remove</param>
    /// <returns>Result of the operation</returns>
    PermissionModifyResult RemovePermissionFromClient(IGameClient client, string permission);

    // ========== Group Management ==========

    /// <summary>
    /// Get all admin groups.
    /// </summary>
    /// <returns>Read-only dictionary of group name to group</returns>
    IReadOnlyDictionary<string, ITnmsPlfdAdminGroup> GetGroups();

    /// <summary>
    /// Add client to admin group.
    /// </summary>
    /// <param name="client">Client to add</param>
    /// <param name="groupName">Group name</param>
    /// <returns>Result of the operation</returns>
    PermissionModifyResult AddClientToGroup(IGameClient client, string groupName);

    /// <summary>
    /// Remove client from admin group.
    /// </summary>
    /// <param name="client">Client to remove</param>
    /// <param name="groupName">Group name</param>
    /// <returns>Result of the operation</returns>
    PermissionModifyResult RemoveClientFromGroup(IGameClient client, string groupName);

    /// <summary>
    /// Add permission to admin group.
    /// </summary>
    /// <param name="groupName">Group name</param>
    /// <param name="permission">Permission to add</param>
    /// <returns>Result of the operation</returns>
    PermissionModifyResult AddPermissionToGroup(string groupName, string permission);

    /// <summary>
    /// Remove permission from admin group.
    /// </summary>
    /// <param name="groupName">Group name</param>
    /// <param name="permission">Permission to remove</param>
    /// <returns>Result of the operation</returns>
    PermissionModifyResult RemovePermissionFromGroup(string groupName, string permission);
}
