using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Data;

public class AdminServerRepository : IAdminServerRepository
{
    private readonly AdministrationDbContext _context;

    public AdminServerRepository(AdministrationDbContext context)
    {
        _context = context;
    }

    public async Task<TnmsAdminServer?> GetByIdAsync(string serverName)
    {
        return await _context.AdminServers
            .Include(s => s.UserPermissionServers)
            .Include(s => s.GroupPermissionServers)
            .FirstOrDefaultAsync(s => s.ServerName == serverName);
    }

    public async Task<IEnumerable<TnmsAdminServer>> GetAllAsync()
    {
        return await _context.AdminServers
            .Include(s => s.UserPermissionServers)
            .Include(s => s.GroupPermissionServers)
            .ToListAsync();
    }

    public async Task<TnmsAdminServer> AddAsync(TnmsAdminServer server)
    {
        _context.AdminServers.Add(server);
        await _context.SaveChangesAsync();
        return server;
    }

    public async Task UpdateAsync(TnmsAdminServer server)
    {
        _context.AdminServers.Update(server);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string serverName)
    {
        var server = await _context.AdminServers.FindAsync(serverName);
        if (server != null)
        {
            _context.AdminServers.Remove(server);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(TnmsAdminServer server)
    {
        _context.AdminServers.Remove(server);
        await _context.SaveChangesAsync();
    }

    public IQueryable<TnmsAdminServer> Query()
    {
        return _context.AdminServers.AsQueryable();
    }
}
