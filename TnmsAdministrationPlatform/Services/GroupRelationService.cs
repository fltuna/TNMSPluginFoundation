using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Data;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Services;

public class GroupRelationService
{
    private readonly IGroupRelationRepository _relationRepository;
    
    public GroupRelationService(IGroupRelationRepository relationRepository)
    {
        _relationRepository = relationRepository;
    }
    
    public async Task<bool> AddUserToGroupAsync(ulong steamId, int groupId)
    {
        var existingRelation = await _relationRepository.Query()
            .FirstOrDefaultAsync(gr => gr.UserSteamId == steamId && gr.GroupId == groupId);
        
        if (existingRelation != null)
            return false; // User is already in the group
        
        var newRelation = new TnmsGroupRelation
        {
            UserSteamId = steamId,
            GroupId = groupId,
            CreatedAt = DateTime.UtcNow
        };
        
        await _relationRepository.AddUserToGroupAsync(newRelation);
        return true;
    }
    
    public async Task<bool> RemoveUserFromGroupAsync(ulong steamId, int groupId)
    {
        await _relationRepository.RemoveUserFromGroupAsync(steamId, groupId);
        return true;
    }
    
    public async Task<List<TnmsGroupRelation>> GetUserGroupsAsync(ulong steamId)
    {
        return (await _relationRepository.GetUserGroupsAsync(steamId)).ToList();
    }
    
    public async Task<List<TnmsGroupRelation>> GetGroupUsersAsync(int groupId)
    {
        return (await _relationRepository.GetGroupUsersAsync(groupId)).ToList();
    }
    
    public async Task<bool> IsUserInGroupAsync(ulong steamId, int groupId)
    {
        return await _relationRepository.IsUserInGroupAsync(steamId, groupId);
    }
    
    public async Task<bool> RemoveUserFromAllGroupsAsync(ulong steamId)
    {
        await _relationRepository.RemoveUserFromAllGroupsAsync(steamId);
        return true;
    }
    
    public async Task<bool> RemoveAllUsersFromGroupAsync(int groupId)
    {
        await _relationRepository.RemoveAllUsersFromGroupAsync(groupId);
        return true;
    }
}
