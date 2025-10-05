using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Data;

public class GroupPermissionRepository : IGroupPermissionRepository
{
    private readonly AdministrationDbContext _context;

    public GroupPermissionRepository(AdministrationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TnmsGroupPermissionGlobal>> GetGlobalPermissionsAsync(int groupId)
    {
        return await _context.GroupPermissionGlobals
            .Where(p => p.GroupId == groupId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TnmsGroupPermissionServer>> GetServerPermissionsAsync(int groupId, string? serverName = null)
    {
        var query = _context.GroupPermissionServers
            .Where(p => p.GroupId == groupId);

        if (!string.IsNullOrEmpty(serverName))
            query = query.Where(p => p.ServerName == serverName);

        return await query.ToListAsync();
    }

    public async Task<TnmsGroupPermissionGlobal> AddGlobalPermissionAsync(TnmsGroupPermissionGlobal permission)
    {
        _context.GroupPermissionGlobals.Add(permission);
        await _context.SaveChangesAsync();
        return permission;
    }

    public async Task<TnmsGroupPermissionServer> AddServerPermissionAsync(TnmsGroupPermissionServer permission)
    {
        _context.GroupPermissionServers.Add(permission);
        await _context.SaveChangesAsync();
        return permission;
    }

    public async Task DeleteGlobalPermissionAsync(int groupId, string permissionNode)
    {
        var permission = await _context.GroupPermissionGlobals
            .FirstOrDefaultAsync(p => p.GroupId == groupId && p.PermissionNode == permissionNode);

        if (permission != null)
        {
            _context.GroupPermissionGlobals.Remove(permission);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteServerPermissionAsync(int groupId, string permissionNode, string serverName)
    {
        var permission = await _context.GroupPermissionServers
            .FirstOrDefaultAsync(p => p.GroupId == groupId && 
                                    p.PermissionNode == permissionNode && 
                                    p.ServerName == serverName);

        if (permission != null)
        {
            _context.GroupPermissionServers.Remove(permission);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearAllGroupPermissionsAsync(int groupId)
    {
        var globalPermissions = await _context.GroupPermissionGlobals
            .Where(p => p.GroupId == groupId)
            .ToListAsync();

        var serverPermissions = await _context.GroupPermissionServers
            .Where(p => p.GroupId == groupId)
            .ToListAsync();

        _context.GroupPermissionGlobals.RemoveRange(globalPermissions);
        _context.GroupPermissionServers.RemoveRange(serverPermissions);
        await _context.SaveChangesAsync();
    }

    public IQueryable<TnmsGroupPermissionGlobal> QueryGlobal()
    {
        return _context.GroupPermissionGlobals.AsQueryable();
    }

    public IQueryable<TnmsGroupPermissionServer> QueryServer()
    {
        return _context.GroupPermissionServers.AsQueryable();
    }
}
