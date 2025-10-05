using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Data;

public interface IAdminUserRepository
{
    Task<TnmsAdminUser?> GetByIdAsync(ulong steamId);
    Task<IEnumerable<TnmsAdminUser>> GetAllAsync();
    Task<TnmsAdminUser> AddAsync(TnmsAdminUser user);
    Task UpdateAsync(TnmsAdminUser user);
    Task DeleteAsync(ulong steamId);
    Task DeleteAsync(TnmsAdminUser user);
    IQueryable<TnmsAdminUser> Query();
}

public interface IAdminServerRepository
{
    Task<TnmsAdminServer?> GetByIdAsync(string serverName);
    Task<IEnumerable<TnmsAdminServer>> GetAllAsync();
    Task<TnmsAdminServer> AddAsync(TnmsAdminServer server);
    Task UpdateAsync(TnmsAdminServer server);
    Task DeleteAsync(string serverName);
    Task DeleteAsync(TnmsAdminServer server);
    IQueryable<TnmsAdminServer> Query();
}

public interface IAdminGroupRepository
{
    Task<TnmsAdminGroup?> GetByIdAsync(int groupId);
    Task<TnmsAdminGroup?> GetByNameAsync(string groupName);
    Task<IEnumerable<TnmsAdminGroup>> GetAllAsync();
    Task<TnmsAdminGroup> AddAsync(TnmsAdminGroup group);
    Task UpdateAsync(TnmsAdminGroup group);
    Task DeleteAsync(int groupId);
    Task DeleteAsync(TnmsAdminGroup group);
    IQueryable<TnmsAdminGroup> Query();
}

public interface IUserPermissionRepository
{
    Task<IEnumerable<TnmsUserPermissionGlobal>> GetGlobalPermissionsAsync(ulong steamId);
    Task<IEnumerable<TnmsUserPermissionServer>> GetServerPermissionsAsync(ulong steamId, string? serverName = null);
    Task<TnmsUserPermissionGlobal> AddGlobalPermissionAsync(TnmsUserPermissionGlobal permission);
    Task<TnmsUserPermissionServer> AddServerPermissionAsync(TnmsUserPermissionServer permission);
    Task DeleteGlobalPermissionAsync(ulong steamId, string permissionNode);
    Task DeleteServerPermissionAsync(ulong steamId, string permissionNode, string serverName);
    Task ClearAllUserPermissionsAsync(ulong steamId);
    IQueryable<TnmsUserPermissionGlobal> QueryGlobal();
    IQueryable<TnmsUserPermissionServer> QueryServer();
}

public interface IGroupPermissionRepository
{
    Task<IEnumerable<TnmsGroupPermissionGlobal>> GetGlobalPermissionsAsync(int groupId);
    Task<IEnumerable<TnmsGroupPermissionServer>> GetServerPermissionsAsync(int groupId, string? serverName = null);
    Task<TnmsGroupPermissionGlobal> AddGlobalPermissionAsync(TnmsGroupPermissionGlobal permission);
    Task<TnmsGroupPermissionServer> AddServerPermissionAsync(TnmsGroupPermissionServer permission);
    Task DeleteGlobalPermissionAsync(int groupId, string permissionNode);
    Task DeleteServerPermissionAsync(int groupId, string permissionNode, string serverName);
    Task ClearAllGroupPermissionsAsync(int groupId);
    IQueryable<TnmsGroupPermissionGlobal> QueryGlobal();
    IQueryable<TnmsGroupPermissionServer> QueryServer();
}

public interface IGroupRelationRepository
{
    Task<IEnumerable<TnmsGroupRelation>> GetUserGroupsAsync(ulong steamId);
    Task<IEnumerable<TnmsGroupRelation>> GetGroupUsersAsync(int groupId);
    Task<bool> IsUserInGroupAsync(ulong steamId, int groupId);
    Task<TnmsGroupRelation> AddUserToGroupAsync(TnmsGroupRelation relation);
    Task RemoveUserFromGroupAsync(ulong steamId, int groupId);
    Task RemoveUserFromAllGroupsAsync(ulong steamId);
    Task RemoveAllUsersFromGroupAsync(int groupId);
    IQueryable<TnmsGroupRelation> Query();
}
