using Chat_App.Models;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;
        public UserRepository(ApplicationDBContext context) { _context = context; }
        public IQueryable<UsersList> Query() => _context.user.AsQueryable();
        public async Task<UsersList?> GetByIdAsync(int id) => await _context.user.FindAsync(id);
        public async Task<UsersList?> GetByEmailAsync(string email)
            => await _context.user.FirstOrDefaultAsync(x => x.email == email);
        public async Task<UsersList?> GetByUsernameOrEmailAsync(string username, string password)
            => await _context.user.FirstOrDefaultAsync(user =>
                (user.username == username || user.email == username) && user.password == password);
        public async Task<bool> ExistsByEmailAsync(string email)
            => await _context.user.AnyAsync(x => x.email == email);
        public async Task<bool> ExistsByUsernameAsync(string username)
            => await _context.user.AnyAsync(x => x.username == username);
        public async Task AddAsync(UsersList user) { _context.user.Add(user); await _context.SaveChangesAsync(); }
        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }
        public async Task<Dictionary<int, UsersList>> GetByIdsDictionaryAsync(List<int> userIds)
            => await _context.user.Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u);
        public async Task<List<UsersList>> GetByFriendIdsAsync(List<int> friendIds)
            => await _context.user.Where(u => friendIds.Contains(u.Id)).ToListAsync();
        public async Task<(List<UsersList> Users, int TotalCount)> SearchExceptAsync(int currentUserId, int skip, int take)
        {
            var query = _context.user.Where(u => u.Id != currentUserId);
            var total = await query.CountAsync();
            var users = await query.OrderBy(u => u.username).Skip(skip).Take(take).ToListAsync();
            return (users, total);
        }
    }
}