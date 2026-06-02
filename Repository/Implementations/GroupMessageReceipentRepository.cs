using Chat_App.Models;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Repositories
{
    public class GroupMessageRecipientRepository : IGroupMessageRecipientRepository
    {
        private readonly ApplicationDBContext _context;
        public GroupMessageRecipientRepository(ApplicationDBContext context) { _context = context; }
        public IQueryable<GroupMessageRecipient> Query() => _context.groupMessageRecipient.AsQueryable();
        public async Task<GroupMessageRecipient?> GetByMessageAndUserAsync(int messageId, int userId)
            => await _context.groupMessageRecipient.FirstOrDefaultAsync(r => r.GroupMessageId == messageId && r.UserId == userId);
        public async Task<List<GroupMessageRecipient>> GetByMessageIdAsync(int messageId)
            => await _context.groupMessageRecipient.Where(r => r.GroupMessageId == messageId).ToListAsync();
        public async Task<int> GetDeliveredCountAsync(int messageId)
            => await _context.groupMessageRecipient.CountAsync(r => r.GroupMessageId == messageId && r.IsDelivered);
        public async Task<int> GetReadCountAsync(int messageId)
            => await _context.groupMessageRecipient.CountAsync(r => r.GroupMessageId == messageId && r.IsRead);
        public async Task<Dictionary<int, int>> GetGroupUnreadCountsAsync(int userId, List<int> groupIds)
            => await (from r in _context.groupMessageRecipient
                      join m in _context.groupMessage on r.GroupMessageId equals m.GroupMessageId
                      where r.UserId == userId && !r.IsRead &&
                            m.DeletedStatus != "EveryOne" &&
                            !(m.DeletedStatus == "Forme" && m.DeletedForUserId == userId) &&
                            m.SenderId != userId && groupIds.Contains(m.GroupId)
                      group r by m.GroupId into g
                      select new { GroupId = g.Key, Count = g.Count() })
                     .ToDictionaryAsync(x => x.GroupId, x => x.Count);
        public async Task AddAsync(GroupMessageRecipient recipient) { _context.groupMessageRecipient.Add(recipient); await _context.SaveChangesAsync(); }
        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }
    }
}