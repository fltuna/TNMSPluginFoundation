using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsLocalizationPlatform.Data;
using TnmsLocalizationPlatform.Models;
using TnmsCentralizedDbPlatform.Shared;

namespace TnmsLocalizationPlatform.Services;

public class UserLanguageService
{
    private readonly LocalizationDbContext _context;
    private readonly ITnmsRepository<UserLanguage> _repository;
    
    public UserLanguageService(LocalizationDbContext context, ITnmsRepository<UserLanguage> repository)
    {
        _context = context;
        _repository = repository;
    }
    
    public async Task<UserLanguage?> GetUserLanguageAsync(long steamId)
    {
        return await _repository.Query()
            .FirstOrDefaultAsync(ul => ul.SteamId == steamId);
    }
    
    public async Task<string?> GetUserLanguageCodeAsync(long steamId)
    {
        var userLanguage = await GetUserLanguageAsync(steamId);
        return userLanguage?.LanguageCode;
    }
    
    public async Task<UserLanguage> SaveUserLanguageAsync(long steamId, string languageCode)
    {
        var existingUserLanguage = await GetUserLanguageAsync(steamId);
        
        if (existingUserLanguage != null)
        {
            existingUserLanguage.LanguageCode = languageCode;
            existingUserLanguage.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(existingUserLanguage);
        }
        else
        {
            var newUserLanguage = new UserLanguage
            {
                SteamId = steamId,
                LanguageCode = languageCode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _repository.AddAsync(newUserLanguage);
            existingUserLanguage = newUserLanguage;
        }
        
        await _context.SaveChangesAsync();
        return existingUserLanguage;
    }
    
    public async Task<bool> DeleteUserLanguageAsync(long steamId)
    {
        var userLanguage = await GetUserLanguageAsync(steamId);
        if (userLanguage == null)
            return false;
            
        _context.UserLanguages.Remove(userLanguage);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<Dictionary<string, int>> GetLanguageStatisticsAsync()
    {
        return await _repository.Query()
            .GroupBy(ul => ul.LanguageCode)
            .Select(g => new { LanguageCode = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.LanguageCode, x => x.Count);
    }
}
