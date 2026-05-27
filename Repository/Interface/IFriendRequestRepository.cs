using Chat_App.Models;
namespace Chat_App.Repositories
{
    public interface IFriendRequestRepository
    {
        Task<FriendRequest?> GetFriendshipAsync(int userId1, int userId2);
        Task<FriendRequest?> GetByIdAndReceiverAsync(int requestId, int receiverId);
        Task<List<int>> GetAcceptedFriendIdsAsync(int userId);
        Task<List<FriendRequest>> GetPendingForReceiverAsync(int receiverId);
        Task<List<FriendRequest>> GetPendingBySenderAsync(int senderId);
        Task<List<FriendRequest>> GetFriendshipsForUsersAsync(int currentUserId, List<int> userIds);
        Task<FriendRequest?> GetByUserIdsAsync(int userId1, int userId2);
        Task AddAsync(FriendRequest request);
        Task SaveChangesAsync();
        IQueryable<FriendRequest> Query();
        Task<FriendRequest?> GetByIdAndSenderAsync(int requestId, int senderId);
        Task DeleteAsync(FriendRequest request);
    }
}