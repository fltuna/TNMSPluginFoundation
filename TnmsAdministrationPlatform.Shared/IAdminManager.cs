using System.Threading.Tasks;
using Sharp.Shared.Objects;

namespace TnmsAdministrationPlatform.Shared;

public interface IAdminManager
{
    public const string RootPermissionWildCard = "*";
    public const string AdminPermissionNode = "tnms.admin";
    public const string ModSharpModuleIdentity = "TnmsAdministrationPlatform";
    
    /// <summary>
    /// Client has permission?
    /// </summary>
    /// <param name="client">Client to check</param>
    /// <param name="permission">Permission node, e.g. tnms.permisson.node</param>
    /// <returns>True if player has permissions</returns>
    public bool ClientHasPermission(IGameClient? client, string permission);
    
    /// <summary>
    /// Check executor can target the client
    /// </summary>
    /// <param name="executor"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool ClientCanTarget(IGameClient? executor, IGameClient target);

    /// <summary>
    /// Add permission to client for specific server or globally
    /// </summary>
    /// <param name="client">Client to check</param>
    /// <param name="permission">Permission node, e.g. tnms.permisson.node</param>
    /// <param name="serverName">Server name for server-specific permission, null for global permission</param>
    /// <returns>
    /// Success if permission was added to client <br/>
    /// FailureDuplicatePermission if client already had permission <br/>
    /// </returns>
    public PermissionSaveResult AddPermissionToClient(IGameClient client, string permission, string? serverName = null);

    /// <summary>
    /// Remove permission from client for specific server or globally
    /// </summary>
    /// <param name="client">Client to check</param>
    /// <param name="permission">Permission node, e.g. tnms.permisson.node</param>
    /// <param name="serverName">Server name for server-specific permission, null for global permission</param>
    /// <returns>
    /// Success if permission was removed from client <br/>
    /// FailureDontHavePermission if client did not have permission <br/>
    /// </returns>
    public PermissionSaveResult RemovePermissionFromClient(IGameClient client, string permission, string? serverName = null);
    
    /// <summary>
    /// Add permission to group for specific server or globally
    /// </summary>
    /// <param name="groupName"></param>
    /// <param name="permission"></param>
    /// <param name="serverName">Server name for server-specific permission, null for global permission</param>
    /// <returns>
    /// GroupNotFound if no group matches with groupName <br/>
    /// Success if permission was added to group <br/>
    /// FailureDuplicatePermission if group already had permission <br/>
    /// </returns>
    public PermissionSaveResult AddPermissionToGroup(string groupName, string permission, string? serverName = null);
    
    /// <summary>
    /// Remove permission from group for specific server or globally
    /// </summary>
    /// <param name="groupName"></param>
    /// <param name="permission"></param>
    /// <param name="serverName">Server name for server-specific permission, null for global permission</param>
    /// <returns>
    /// GroupNotFound if no group matches with groupName <br/>
    /// Success if permission was removed from group <br/>
    /// FailureDontHavePermission if group did not have permission <br/>
    /// </returns>
    public PermissionSaveResult RemovePermissionFromGroup(string groupName, string permission, string? serverName = null);
    
    /// <summary>
    /// Add client to admin group
    /// </summary>
    /// <param name="client"></param>
    /// <param name="groupName"></param>
    /// <returns>
    /// GroupNotFound if no group matches with groupName <br/>
    /// Success if client was added to group <br/>
    /// FailureClientAlreadyInGroup if client was already in group <br/>
    /// </returns>
    public PermissionSaveResult AddClientToGroup(IGameClient client, string groupName);
    
    /// <summary>
    /// Remove client from admin group
    /// </summary>
    /// <param name="client"></param>
    /// <param name="groupName"></param>
    /// <returns>
    /// GroupNotFound if no group matches with groupName <br/>
    /// Success if client was removed from group <br/>
    /// FailureClientDontHaveGroup if client was not in group <br/>
    /// </returns>
    public PermissionSaveResult RemoveClientFromGroup(IGameClient client, string groupName);

    /// <summary>
    /// Get admin information associated with specified client
    /// </summary>
    /// <param name="client"></param>
    /// <returns>Null, if client is bot or failed to fetch</returns>
    public IAdminUser? GetAdminInformation(IGameClient client);

    /// <summary>
    /// Get immunity level of specified client
    /// </summary>
    /// <param name="client">Client to get immunity from</param>
    /// <returns>Immunity level (0-255)</returns>
    public byte GetClientImmunity(IGameClient client);

    /// <summary>
    /// Set immunity level of specified client
    /// </summary>
    /// <param name="client">Client to set immunity for</param>
    /// <param name="immunity">Immunity level (0-255)</param>
    /// <returns>
    /// Success if immunity was set successfully <br/>
    /// FailureGeneral if there was an error setting immunity <br/>
    /// </returns>
    public PermissionSaveResult SetClientImmunity(IGameClient client, byte immunity);
}