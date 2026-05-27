using Chat_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Repositories
{
    public class MomentRepository : IMomentRepository
    {
        private readonly ApplicationDBContext _context;
        public MomentRepository(ApplicationDBContext context) { _context = context; }
        public async Task<List<Moment>> GetActiveMomentsAsync(List<int> userIds)
        {
            var now = DateTime.UtcNow;
            return await _context.moments
                .Include(m => m.Views)
                .Where(m => userIds.Contains(m.UserId) && m.ExpiresAt > now)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<Moment>> GetMyMomentsAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _context.moments
                .Include(m => m.Views)
                .Where(m => m.UserId == userId && m.ExpiresAt > now)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
        public async Task<MomentView?> GetExistingViewAsync(int momentId, int viewerId)
            => await _context.momentView.FirstOrDefaultAsync(v => v.MomentId == momentId && v.ViewedByUserId == viewerId);
        public async Task AddMomentAsync(Moment moment) { _context.moments.Add(moment); await _context.SaveChangesAsync(); }
        public async Task AddViewAsync(MomentView view) { _context.momentView.Add(view); await _context.SaveChangesAsync(); }
        public async Task<int> GetViewCountAsync(int momentId)
            => await _context.momentView.CountAsync(v => v.MomentId == momentId);
        public async Task<List<object>> GetMomentViewsAsync(int momentId)
            => await _context.momentView
                .Where(v => v.MomentId == momentId)
                .Join(_context.user, v => v.ViewedByUserId, u => u.Id, (v, u) => new
                {
                    u.Id,
                    u.username,
                    u.ProfileImagePath,
                    v.ViewedAt
                })
                .OrderByDescending(x => x.ViewedAt)
                .ToListAsync<object>();
        public async Task<List<Moment>> GetExpiredMomentsAsync()
            => await _context.moments.Where(m => m.ExpiresAt <= DateTime.UtcNow).ToListAsync();
        public void RemoveMoment(Moment moment) { _context.moments.Remove(moment); }
        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }
        public async Task<Moment?> GetMomentByIdAsync(int momentId)
           => await _context.moments.FirstOrDefaultAsync(moment => moment.Id == momentId);
    }
}