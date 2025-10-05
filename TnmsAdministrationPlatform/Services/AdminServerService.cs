using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Data;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Services;

public class AdminServerService
{
    private readonly IAdminServerRepository _serverRepository;
    
    public AdminServerService(IAdminServerRepository serverRepository)
    {
        _serverRepository = serverRepository;
    }
    
    public async Task<TnmsAdminServer?> GetServerAsync(string serverName)
    {
        return await _serverRepository.GetByIdAsync(serverName);
    }
    
    public async Task<TnmsAdminServer> CreateOrUpdateServerAsync(string serverName, string? description = null, bool isActive = true)
    {
        var existingServer = await _serverRepository.Query()
            .FirstOrDefaultAsync(s => s.ServerName == serverName);
        
        if (existingServer != null)
        {
            existingServer.Description = description;
            existingServer.IsActive = isActive;
            existingServer.UpdatedAt = DateTime.UtcNow;
            await _serverRepository.UpdateAsync(existingServer);
            return existingServer;
        }
        else
        {
            var newServer = new TnmsAdminServer
            {
                ServerName = serverName,
                Description = description,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            return await _serverRepository.AddAsync(newServer);
        }
    }
    
    public async Task<bool> DeleteServerAsync(string serverName)
    {
        await _serverRepository.DeleteAsync(serverName);
        return true;
    }
    
    public async Task<List<TnmsAdminServer>> GetAllServersAsync()
    {
        return (await _serverRepository.GetAllAsync()).ToList();
    }
    
    public async Task<List<TnmsAdminServer>> GetActiveServersAsync()
    {
        return (await _serverRepository.GetAllAsync())
            .Where(s => s.IsActive)
            .ToList();
    }
    
    public async Task<bool> SetServerActiveStatusAsync(string serverName, bool isActive)
    {
        var server = await _serverRepository.Query()
            .FirstOrDefaultAsync(s => s.ServerName == serverName);
        
        if (server == null)
            return false;
        
        server.IsActive = isActive;
        server.UpdatedAt = DateTime.UtcNow;
        await _serverRepository.UpdateAsync(server);
        return true;
    }
}
