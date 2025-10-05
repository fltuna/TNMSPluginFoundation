using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Data;

public class GroupRelationRepository : IGroupRelationRepository
{
    private readonly AdministrationDbContext _context;

    public GroupRelationRepository(AdministrationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TnmsGroupRelation>> GetUserGroupsAsync(ulong steamId)
    {
        return await _context.GroupRelations
            .Include(gr => gr.Group)
                .ThenInclude(g => g.GroupPermissionGlobals)
            .Include(gr => gr.Group)
                .ThenInclude(g => g.GroupPermissionServers)
            .Where(gr => gr.UserSteamId == steamId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TnmsGroupRelation>> GetGroupUsersAsync(int groupId)
    {
        return await _context.GroupRelations
            .Include(gr => gr.User)
            .Where(gr => gr.GroupId == groupId)
            .ToListAsync();
    }

    public async Task<bool> IsUserInGroupAsync(ulong steamId, int groupId)
    {
        return await _context.GroupRelations
            .AnyAsync(gr => gr.UserSteamId == steamId && gr.GroupId == groupId);
    }

    public async Task<TnmsGroupRelation> AddUserToGroupAsync(TnmsGroupRelation relation)
    {
        _context.GroupRelations.Add(relation);
        await _context.SaveChangesAsync();
        return relation;
    }

    public async Task RemoveUserFromGroupAsync(ulong steamId, int groupId)
    {
        var relation = await _context.GroupRelations
            .FirstOrDefaultAsync(gr => gr.UserSteamId == steamId && gr.GroupId == groupId);

        if (relation != null)
        {
            _context.GroupRelations.Remove(relation);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveUserFromAllGroupsAsync(ulong steamId)
    {
        var relations = await _context.GroupRelations
            .Where(gr => gr.UserSteamId == steamId)
            .ToListAsync();

        _context.GroupRelations.RemoveRange(relations);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAllUsersFromGroupAsync(int groupId)
    {
        var relations = await _context.GroupRelations
            .Where(gr => gr.GroupId == groupId)
            .ToListAsync();

        _context.GroupRelations.RemoveRange(relations);
        await _context.SaveChangesAsync();
    }

    public IQueryable<TnmsGroupRelation> Query()
    {
        return _context.GroupRelations.AsQueryable();
    }
}
