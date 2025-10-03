using Sharp.Shared.Objects;

namespace TnmsAdministrationPlatform;

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
    /// Add permission to client
    /// </summary>
    /// <param name="client">Client to check</param>
    /// <param name="permission">Permission node, e.g. tnms.permisson.node</param>
    /// <returns>true if permission is successfully added to client, false if player already have specified permission</returns>
    public bool AddPermissionToClient(IGameClient client, string permission);
    
    
    /// <summary>
    /// Remove permission from client
    /// </summary>
    /// <param name="client">Client to check</param>
    /// <param name="permission">Permission node, e.g. tnms.permisson.node</param>
    /// <returns>true if permission is successfully removed from client, false if player don't have specified permission</returns>
    public bool RemovePermissionFromClient(IGameClient client, string permission);
    
    /// <summary>
    /// Add client to admin group
    /// </summary>
    /// <param name="client"></param>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public bool AddClientToGroup(IGameClient client, string groupName);
    
    /// <summary>
    /// Remove client from admin group
    /// </summary>
    /// <param name="client"></param>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public bool RemoveClientFromGroup(IGameClient client, string groupName);
    
    /// <summary>
    /// Get admin information associated with specified client
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    public IAdminUser GetAdminInformation(IGameClient client);
}