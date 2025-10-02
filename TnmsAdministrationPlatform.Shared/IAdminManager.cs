using Sharp.Shared.Objects;

namespace TnmsAdministrationPlatform;

public interface IAdminManager
{
    public const string RootPermissionNode = "tnms.root";
    public const string ModSharpModuleIdentity = "TnmsAdministrationPlatform";
    
    /// <summary>
    /// Get Admin manager from shared insta
    /// </summary>
    public delegate IAdminManager GetAdminManager();
    
    /// <summary>
    /// Client has permission?
    /// </summary>
    /// <param name="client">Client to check</param>
    /// <param name="permission">Permission node, e.g. tnms.permisson.node</param>
    /// <returns>True if player has permissions</returns>
    public bool ClientHasPermission(IGameClient? client, string permission);
    
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
}