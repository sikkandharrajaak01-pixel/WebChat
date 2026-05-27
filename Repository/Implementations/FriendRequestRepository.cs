using Chat_App.Models;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Repositories
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly ApplicationDBContext _context;
        public FriendRequestRepository(ApplicationDBContext context) { _context = context; }
        public IQueryable<FriendRequest> Query() => _context.friendRequests.AsQueryable();
        public async Task<FriendRequest?> GetFriendshipAsync(int userId1, int userId2)
            => await _context.friendRequests.FirstOrDefaultAsync(f =>
                (f.SenderId == userId1 && f.ReceiverId == userId2) ||
                (f.SenderId == userId2 && f.ReceiverId == userId1));
        public async Task<FriendRequest?> GetByIdAndReceiverAsync(int requestId, int receiverId)
            => await _context.friendRequests.FirstOrDefaultAsync(f => f.Id == requestId && f.ReceiverId == receiverId && f.Status == "Pending");
        public async Task<List<int>> GetAcceptedFriendIdsAsync(int userId)
            => await _context.friendRequests
                .Where(f => (f.SenderId == userId || f.ReceiverId == userId) && f.Status == "Accepted")
                .Select(f => f.SenderId == userId ? f.ReceiverId : f.SenderId)
                .ToListAsync();
        public async Task<List<FriendRequest>> GetPendingForReceiverAsync(int receiverId)
            => await _context.friendRequests
                .Where(f => f.ReceiverId == receiverId && f.Status == "Pending")
                .OrderByDescending(f => f.SentAt)
                .ToListAsync();
        public async Task<List<FriendRequest>> GetPendingBySenderAsync(int senderId)
            => await _context.friendRequests
                .Where(f => f.SenderId == senderId && f.Status == "Pending")
                .OrderByDescending(f => f.SentAt)
                .ToListAsync();
        public async Task<List<FriendRequest>> GetFriendshipsForUsersAsync(int currentUserId, List<int> userIds)
            => await _context.friendRequests
                .Where(f => (f.SenderId == currentUserId || f.ReceiverId == currentUserId) &&
                            userIds.Contains(f.SenderId == currentUserId ? f.ReceiverId : f.SenderId))
                .ToListAsync();
        public async Task<FriendRequest?> GetByUserIdsAsync(int userId1, int userId2)
            => await _context.friendRequests.FirstOrDefaultAsync(f =>
                (f.SenderId == userId1 && f.ReceiverId == userId2) ||
                (f.SenderId == userId2 && f.ReceiverId == userId1));
        public async Task AddAsync(FriendRequest request) { _context.friendRequests.Add(request); await _context.SaveChangesAsync(); }
        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }

        public async Task<FriendRequest?> GetByIdAndSenderAsync(int requestId, int senderId)
            => await _context.friendRequests.FirstOrDefaultAsync(request => request.SenderId == senderId && request.Id == requestId && request.Status == "Pending");
        public async Task DeleteAsync(FriendRequest request)
        {
            _context.friendRequests.Remove(request);
            await _context.SaveChangesAsync();
        }
    }
}