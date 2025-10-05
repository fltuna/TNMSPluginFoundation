using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Data;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Services;

public class AdminGroupService
{
    private readonly IAdminGroupRepository _groupRepository;
    
    public AdminGroupService(IAdminGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public async Task<TnmsAdminGroup?> GetAdminGroupAsync(int groupId)
    {
        return await _groupRepository.GetByIdAsync(groupId);
    }
    
    public async Task<TnmsAdminGroup?> GetAdminGroupByNameAsync(string groupName)
    {
        return await _groupRepository.GetByNameAsync(groupName);
    }
    
    public async Task<TnmsAdminGroup> CreateAdminGroupAsync(string groupName, string? description = null)
    {
        var newGroup = new TnmsAdminGroup
        {
            GroupName = groupName,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
        
        return await _groupRepository.AddAsync(newGroup);
    }
    
    public async Task<TnmsAdminGroup?> UpdateAdminGroupAsync(int groupId, string? newName = null, string? newDescription = null)
    {
        var group = await _groupRepository.Query()
            .FirstOrDefaultAsync(g => g.Id == groupId);
        
        if (group == null)
            return null;
        
        if (!string.IsNullOrEmpty(newName))
            group.GroupName = newName;
        
        if (newDescription != null)
            group.Description = newDescription;
        
        await _groupRepository.UpdateAsync(group);
        return group;
    }
    
    public async Task<bool> DeleteAdminGroupAsync(int groupId)
    {
        await _groupRepository.DeleteAsync(groupId);
        return true;
    }
    
    public async Task<List<TnmsAdminGroup>> GetAllAdminGroupsAsync()
    {
        return (await _groupRepository.GetAllAsync()).ToList();
    }
}
