using Chat_App.Models;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDBContext _context;
        public MessageRepository(ApplicationDBContext context) { _context = context; }
        public IQueryable<Message> Query() => _context.message.AsQueryable();
        public async Task<Message?> GetByIdAsync(int messageId) => await _context.message.FindAsync(messageId);
        public async Task<List<Message>> GetMessagesAsync(int currentUserId, int receiverId, DateTime? before, int? beforeId, int take)
        {
            var query = _context.message
                .Where(m => ((m.SenderId == currentUserId && m.ReceiverId == receiverId) ||
                             (m.SenderId == receiverId && m.ReceiverId == currentUserId)) &&
                            !(m.BlockedStatus && m.ReceiverId == currentUserId));
            if (before.HasValue && beforeId.HasValue)
                query = query.Where(m => m.SentAt < before.Value || (m.SentAt == before.Value && m.MessageId < beforeId.Value));
            return await query.OrderByDescending(m => m.SentAt).ThenByDescending(m => m.MessageId).Take(take).ToListAsync();
        }
        public async Task<Message?> GetLastMessageAsync(int userId1, int userId2, int currentUserId)
            => await _context.message
                .Where(m => ((m.SenderId == userId1 && m.ReceiverId == userId2) ||
                             (m.SenderId == userId2 && m.ReceiverId == userId1)) &&
                            !(m.BlockedStatus && m.ReceiverId == currentUserId) &&
                            !(m.DeletedStatus == "Forme" && m.DeletedForUserId == currentUserId))
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();
        public async Task<Dictionary<int, int>> GetUnreadCountsAsync(int receiverId)
            => await _context.message
                .Where(m => m.ReceiverId == receiverId && !m.IsRead &&
                            m.DeletedStatus != "EveryOne" &&
                            !(m.DeletedStatus == "Forme" && m.DeletedForUserId == receiverId))
                .GroupBy(m => m.SenderId)
                .Select(g => new { SenderId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SenderId, x => x.Count);
        public async Task<List<Message>> GetStarredMessagesAsync()
            => await _context.message.Where(m => m.IsStared == true).ToListAsync();
        public void Update(Message message) { _context.message.Update(message); }
        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }
    }
}
