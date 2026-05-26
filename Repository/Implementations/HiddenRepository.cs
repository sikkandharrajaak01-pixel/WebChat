using Chat_App.Models;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Repositories
{
    public class HiddenRepository : IHiddenRepository
    {
        private readonly ApplicationDBContext _context;
        public HiddenRepository(ApplicationDBContext context) { _context = context; }
        public async Task<List<int>> GetHiddenUserIdsAsync(int userId)
            => await _context.hiddenChat.Where(h => h.UserId == userId).Select(h => h.HiddenUserId).ToListAsync();
        public async Task<List<int>> GetHiddenGroupIdsAsync(int userId)
            => await _context.hiddenGroup.Where(h => h.UserId == userId).Select(h => h.GroupId).ToListAsync();
        public async Task<HiddenChat?> GetHiddenChatAsync(int userId, int hiddenUserId)
            => await _context.hiddenChat.FirstOrDefaultAsync(h => h.UserId == userId && h.HiddenUserId == hiddenUserId);
        public async Task<HiddenGroup?> GetHiddenGroupAsync(int userId, int groupId)
            => await _context.hiddenGroup.FirstOrDefaultAsync(h => h.UserId == userId && h.GroupId == groupId);
        public void RemoveHiddenChat(HiddenChat chat) { _context.hiddenChat.Remove(chat); }
        public void RemoveHiddenGroup(HiddenGroup group) { _context.hiddenGroup.Remove(group); }
        public async Task AddHiddenChatAsync(HiddenChat chat) { _context.hiddenChat.Add(chat); await _context.SaveChangesAsync(); }
        public async Task AddHiddenGroupAsync(HiddenGroup group) { _context.hiddenGroup.Add(group); await _context.SaveChangesAsync(); }
        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }
    }
}