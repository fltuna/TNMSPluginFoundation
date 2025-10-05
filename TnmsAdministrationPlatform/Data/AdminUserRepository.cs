using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Data;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly AdministrationDbContext _context;

    public AdminUserRepository(AdministrationDbContext context)
    {
        _context = context;
    }

    public async Task<TnmsAdminUser?> GetByIdAsync(ulong steamId)
    {
        return await _context.AdminUsers
            .Include(u => u.UserPermissionGlobals)
            .Include(u => u.UserPermissionServers)
            .Include(u => u.GroupRelations)
                .ThenInclude(gr => gr.Group)
            .FirstOrDefaultAsync(u => u.SteamId == steamId);
    }

    public async Task<IEnumerable<TnmsAdminUser>> GetAllAsync()
    {
        return await _context.AdminUsers
            .Include(u => u.UserPermissionGlobals)
            .Include(u => u.UserPermissionServers)
            .Include(u => u.GroupRelations)
                .ThenInclude(gr => gr.Group)
            .ToListAsync();
    }

    public async Task<TnmsAdminUser> AddAsync(TnmsAdminUser user)
    {
        _context.AdminUsers.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(TnmsAdminUser user)
    {
        _context.AdminUsers.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ulong steamId)
    {
        var user = await _context.AdminUsers.FindAsync(steamId);
        if (user != null)
        {
            _context.AdminUsers.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(TnmsAdminUser user)
    {
        _context.AdminUsers.Remove(user);
        await _context.SaveChangesAsync();
    }

    public IQueryable<TnmsAdminUser> Query()
    {
        return _context.AdminUsers.AsQueryable();
    }
}
