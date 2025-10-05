using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Data;

public class AdminGroupRepository : IAdminGroupRepository
{
    private readonly AdministrationDbContext _context;

    public AdminGroupRepository(AdministrationDbContext context)
    {
        _context = context;
    }

    public async Task<TnmsAdminGroup?> GetByIdAsync(int groupId)
    {
        return await _context.AdminGroups
            .Include(g => g.GroupPermissionGlobals)
            .Include(g => g.GroupPermissionServers)
            .Include(g => g.GroupRelations)
                .ThenInclude(gr => gr.User)
            .FirstOrDefaultAsync(g => g.Id == groupId);
    }

    public async Task<TnmsAdminGroup?> GetByNameAsync(string groupName)
    {
        return await _context.AdminGroups
            .Include(g => g.GroupPermissionGlobals)
            .Include(g => g.GroupPermissionServers)
            .Include(g => g.GroupRelations)
                .ThenInclude(gr => gr.User)
            .FirstOrDefaultAsync(g => g.GroupName == groupName);
    }

    public async Task<IEnumerable<TnmsAdminGroup>> GetAllAsync()
    {
        return await _context.AdminGroups
            .Include(g => g.GroupPermissionGlobals)
            .Include(g => g.GroupPermissionServers)
            .Include(g => g.GroupRelations)
                .ThenInclude(gr => gr.User)
            .ToListAsync();
    }

    public async Task<TnmsAdminGroup> AddAsync(TnmsAdminGroup group)
    {
        _context.AdminGroups.Add(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task UpdateAsync(TnmsAdminGroup group)
    {
        _context.AdminGroups.Update(group);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int groupId)
    {
        var group = await _context.AdminGroups.FindAsync(groupId);
        if (group != null)
        {
            _context.AdminGroups.Remove(group);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(TnmsAdminGroup group)
    {
        _context.AdminGroups.Remove(group);
        await _context.SaveChangesAsync();
    }

    public IQueryable<TnmsAdminGroup> Query()
    {
        return _context.AdminGroups.AsQueryable();
    }
}
