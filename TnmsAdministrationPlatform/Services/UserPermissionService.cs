using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Data;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Services;

public class UserPermissionService
{
    private readonly IUserPermissionRepository _permissionRepository;
    
    public UserPermissionService(IUserPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }
    
    // Global permissions
    public async Task<bool> AddGlobalPermissionAsync(ulong steamId, string permissionNode)
    {
        var existingPermission = await _permissionRepository.QueryGlobal()
            .FirstOrDefaultAsync(p => p.UserSteamId == steamId && p.PermissionNode == permissionNode);
        
        if (existingPermission != null)
            return false; // Permission already exists
        
        var newPermission = new TnmsUserPermissionGlobal
        {
            UserSteamId = steamId,
            PermissionNode = permissionNode,
            CreatedAt = DateTime.UtcNow
        };
        
        await _permissionRepository.AddGlobalPermissionAsync(newPermission);
        return true;
    }
    
    public async Task<bool> RemoveGlobalPermissionAsync(ulong steamId, string permissionNode)
    {
        await _permissionRepository.DeleteGlobalPermissionAsync(steamId, permissionNode);
        return true;
    }
    
    public async Task<List<TnmsUserPermissionGlobal>> GetUserGlobalPermissionsAsync(ulong steamId)
    {
        return (await _permissionRepository.GetGlobalPermissionsAsync(steamId)).ToList();
    }
    
    // Server-specific permissions
    public async Task<bool> AddServerPermissionAsync(ulong steamId, string permissionNode, string serverName)
    {
        var existingPermission = await _permissionRepository.QueryServer()
            .FirstOrDefaultAsync(p => p.UserSteamId == steamId && 
                                    p.PermissionNode == permissionNode && 
                                    p.ServerName == serverName);
        
        if (existingPermission != null)
            return false; // Permission already exists
        
        var newPermission = new TnmsUserPermissionServer
        {
            UserSteamId = steamId,
            PermissionNode = permissionNode,
            ServerName = serverName,
            CreatedAt = DateTime.UtcNow
        };
        
        await _permissionRepository.AddServerPermissionAsync(newPermission);
        return true;
    }
    
    public async Task<bool> RemoveServerPermissionAsync(ulong steamId, string permissionNode, string serverName)
    {
        await _permissionRepository.DeleteServerPermissionAsync(steamId, permissionNode, serverName);
        return true;
    }
    
    public async Task<List<TnmsUserPermissionServer>> GetUserServerPermissionsAsync(ulong steamId, string? serverName = null)
    {
        return (await _permissionRepository.GetServerPermissionsAsync(steamId, serverName)).ToList();
    }
    
    public async Task<bool> ClearAllUserPermissionsAsync(ulong steamId)
    {
        await _permissionRepository.ClearAllUserPermissionsAsync(steamId);
        return true;
    }
}
