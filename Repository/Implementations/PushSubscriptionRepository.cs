using Chat_App.Models;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Repositories
{
    public class PushSubscriptionRepository : IPushSubscriptionRepository
    {
        private readonly ApplicationDBContext _context;
        public PushSubscriptionRepository(ApplicationDBContext context) { _context = context; }
        public async Task<PushSubscription?> GetByUserIdAndEndpointAsync(int userId, string endpoint)
            => await _context.pushSubscription.FirstOrDefaultAsync(s => s.UserId == userId && s.Endpoint == endpoint);
        public async Task AddAsync(PushSubscription subscription) { _context.pushSubscription.Add(subscription); await _context.SaveChangesAsync(); }
        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }
    }
}