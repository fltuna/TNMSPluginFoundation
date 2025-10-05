using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Data;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Services;

public class AdminUserService
{
    private readonly IAdminUserRepository _userRepository;
    
    public AdminUserService(IAdminUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<TnmsAdminUser?> GetAdminUserAsync(ulong steamId)
    {
        return await _userRepository.GetByIdAsync(steamId);
    }
    
    public async Task<TnmsAdminUser> CreateOrUpdateAdminUserAsync(ulong steamId, byte immunity = 0)
    {
        var existingUser = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.SteamId == steamId);
        
        if (existingUser != null)
        {
            existingUser.Immunity = immunity;
            existingUser.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(existingUser);
            return existingUser;
        }
        else
        {
            var newUser = new TnmsAdminUser
            {
                SteamId = steamId,
                Immunity = immunity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(newUser);
            return newUser;
        }
    }
    
    public async Task<bool> DeleteAdminUserAsync(ulong steamId)
    {
        await _userRepository.DeleteAsync(steamId);
        return true;
    }
    
    public async Task<List<TnmsAdminUser>> GetAllAdminUsersAsync()
    {
        return (await _userRepository.GetAllAsync()).ToList();
    }
}
