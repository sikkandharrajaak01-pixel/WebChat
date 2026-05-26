using Chat_App.Models;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Repositories
{
    public class ChatBackgroundRepository : IChatBackgroundRepository
    {
        private readonly ApplicationDBContext _context;
        public ChatBackgroundRepository(ApplicationDBContext context) { _context = context; }
        public async Task<ChatBackground?> GetByUserAndPeerAsync(int userId, int peerId)
            => await _context.chatBackground.FirstOrDefaultAsync(b => b.UserId == userId && b.PeerId == peerId);
        public async Task AddAsync(ChatBackground background) { _context.chatBackground.Add(background); await _context.SaveChangesAsync(); }
        public void Remove(ChatBackground background) { _context.chatBackground.Remove(background); }
        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }
    }
}