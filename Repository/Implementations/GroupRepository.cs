using Chat_App.Models;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDBContext _context;
        public GroupRepository(ApplicationDBContext context) { _context = context; }
        public IQueryable<GroupCredentials> Query() => _context.group.AsQueryable();
        public async Task<GroupCredentials?> GetByIdAsync(int groupId) => await _context.group.FindAsync(groupId);
        public async Task<List<GroupCredentials>> GetUserGroupsAsync(int userId)
            => await _context.group.Where(g => g.UserIds.Contains(userId)).ToListAsync();
        public async Task AddAsync(GroupCredentials group) { _context.group.Add(group); await _context.SaveChangesAsync(); }
        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }
    }
}