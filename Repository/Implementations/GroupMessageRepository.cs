using Chat_App.Models;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Repositories
{
    public class GroupMessageRepository : IGroupMessageRepository
    {
        private readonly ApplicationDBContext _context;
        public GroupMessageRepository(ApplicationDBContext context) { _context = context; }
        public IQueryable<GroupMessage> Query() => _context.groupMessage.AsQueryable();
        public async Task<GroupMessage?> GetByIdAsync(int messageId) => await _context.groupMessage.FindAsync(messageId);
        public async Task<List<GroupMessage>> GetGroupMessagesAsync(int groupId, int currentUserId, DateTime? before, int? beforeId, int take)
        {
            var query = _context.groupMessage
                .Where(m => m.GroupId == groupId && (m.DeletedStatus != "Forme" || m.DeletedForUserId != currentUserId));
            if (before.HasValue && beforeId.HasValue)
                query = query.Where(m => m.SentAt < before.Value || (m.SentAt == before.Value && m.GroupMessageId < beforeId.Value));
            return await query.OrderByDescending(m => m.SentAt).ThenByDescending(m => m.GroupMessageId).Take(take).ToListAsync();
        }
        public async Task<GroupMessage?> GetLastMessageAsync(int groupId, int currentUserId)
            => await _context.groupMessage
                .Where(m => m.GroupId == groupId && !(m.DeletedStatus == "Forme" && m.DeletedForUserId == currentUserId))
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();
        public async Task<List<GroupMessage>> GetStarredGroupMessagesAsync()
            => await _context.groupMessage.Where(m => m.IsStared == true).ToListAsync();
        public void Update(GroupMessage message) { _context.groupMessage.Update(message); }
        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }
        public async Task<Dictionary<int, (int Delivered, int Read)>> GetCountsForMessagesAsync(List<int> messageIds)
        {
            var rows = await _context.groupMessageRecipient
                .Where(g => messageIds.Contains(g.GroupMessageId))
                .GroupBy(g => g.GroupMessageId)
                .Select(g => new
                {
                    MessageId = g.Key,
                    Delivered = g.Count(x => x.IsDelivered),
                    Read = g.Count(x => x.IsRead)
                })
                .ToListAsync();

            return rows.ToDictionary(
                r => r.MessageId,
                r => (r.Delivered, r.Read)
            );
        }
    }
}
