using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Data;

public class UserPermissionRepository : IUserPermissionRepository
{
    private readonly AdministrationDbContext _context;

    public UserPermissionRepository(AdministrationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TnmsUserPermissionGlobal>> GetGlobalPermissionsAsync(ulong steamId)
    {
        return await _context.UserPermissionGlobals
            .Where(p => p.UserSteamId == steamId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TnmsUserPermissionServer>> GetServerPermissionsAsync(ulong steamId, string? serverName = null)
    {
        var query = _context.UserPermissionServers
            .Where(p => p.UserSteamId == steamId);

        if (!string.IsNullOrEmpty(serverName))
            query = query.Where(p => p.ServerName == serverName);

        return await query.ToListAsync();
    }

    public async Task<TnmsUserPermissionGlobal> AddGlobalPermissionAsync(TnmsUserPermissionGlobal permission)
    {
        _context.UserPermissionGlobals.Add(permission);
        await _context.SaveChangesAsync();
        return permission;
    }

    public async Task<TnmsUserPermissionServer> AddServerPermissionAsync(TnmsUserPermissionServer permission)
    {
        _context.UserPermissionServers.Add(permission);
        await _context.SaveChangesAsync();
        return permission;
    }

    public async Task DeleteGlobalPermissionAsync(ulong steamId, string permissionNode)
    {
        var permission = await _context.UserPermissionGlobals
            .FirstOrDefaultAsync(p => p.UserSteamId == steamId && p.PermissionNode == permissionNode);

        if (permission != null)
        {
            _context.UserPermissionGlobals.Remove(permission);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteServerPermissionAsync(ulong steamId, string permissionNode, string serverName)
    {
        var permission = await _context.UserPermissionServers
            .FirstOrDefaultAsync(p => p.UserSteamId == steamId && 
                                    p.PermissionNode == permissionNode && 
                                    p.ServerName == serverName);

        if (permission != null)
        {
            _context.UserPermissionServers.Remove(permission);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearAllUserPermissionsAsync(ulong steamId)
    {
        var globalPermissions = await _context.UserPermissionGlobals
            .Where(p => p.UserSteamId == steamId)
            .ToListAsync();

        var serverPermissions = await _context.UserPermissionServers
            .Where(p => p.UserSteamId == steamId)
            .ToListAsync();

        _context.UserPermissionGlobals.RemoveRange(globalPermissions);
        _context.UserPermissionServers.RemoveRange(serverPermissions);
        await _context.SaveChangesAsync();
    }

    public IQueryable<TnmsUserPermissionGlobal> QueryGlobal()
    {
        return _context.UserPermissionGlobals.AsQueryable();
    }

    public IQueryable<TnmsUserPermissionServer> QueryServer()
    {
        return _context.UserPermissionServers.AsQueryable();
    }
}
