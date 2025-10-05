using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Data;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Services;

public class GroupPermissionService
{
    private readonly IGroupPermissionRepository _permissionRepository;
    
    public GroupPermissionService(IGroupPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }
    
    // Global permissions
    public async Task<bool> AddGlobalPermissionAsync(int groupId, string permissionNode)
    {
        var existingPermission = await _permissionRepository.QueryGlobal()
            .FirstOrDefaultAsync(p => p.GroupId == groupId && p.PermissionNode == permissionNode);
        
        if (existingPermission != null)
            return false; // Permission already exists
        
        var newPermission = new TnmsGroupPermissionGlobal
        {
            GroupId = groupId,
            PermissionNode = permissionNode,
            CreatedAt = DateTime.UtcNow
        };
        
        await _permissionRepository.AddGlobalPermissionAsync(newPermission);
        return true;
    }
    
    public async Task<bool> RemoveGlobalPermissionAsync(int groupId, string permissionNode)
    {
        await _permissionRepository.DeleteGlobalPermissionAsync(groupId, permissionNode);
        return true;
    }
    
    public async Task<List<TnmsGroupPermissionGlobal>> GetGroupGlobalPermissionsAsync(int groupId)
    {
        return (await _permissionRepository.GetGlobalPermissionsAsync(groupId)).ToList();
    }
    
    // Server-specific permissions
    public async Task<bool> AddServerPermissionAsync(int groupId, string permissionNode, string serverName)
    {
        var existingPermission = await _permissionRepository.QueryServer()
            .FirstOrDefaultAsync(p => p.GroupId == groupId && 
                                    p.PermissionNode == permissionNode && 
                                    p.ServerName == serverName);
        
        if (existingPermission != null)
            return false; // Permission already exists
        
        var newPermission = new TnmsGroupPermissionServer
        {
            GroupId = groupId,
            PermissionNode = permissionNode,
            ServerName = serverName,
            CreatedAt = DateTime.UtcNow
        };
        
        await _permissionRepository.AddServerPermissionAsync(newPermission);
        return true;
    }
    
    public async Task<bool> RemoveServerPermissionAsync(int groupId, string permissionNode, string serverName)
    {
        await _permissionRepository.DeleteServerPermissionAsync(groupId, permissionNode, serverName);
        return true;
    }
    
    public async Task<List<TnmsGroupPermissionServer>> GetGroupServerPermissionsAsync(int groupId, string? serverName = null)
    {
        return (await _permissionRepository.GetServerPermissionsAsync(groupId, serverName)).ToList();
    }
    
    public async Task<bool> ClearAllGroupPermissionsAsync(int groupId)
    {
        await _permissionRepository.ClearAllGroupPermissionsAsync(groupId);
        return true;
    }
}
