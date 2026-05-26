using Chat_App.Models;
namespace Chat_App.Repositories
{
    public interface IMessageRepository
    {
        Task<Message?> GetByIdAsync(int messageId);
        Task<List<Message>> GetMessagesAsync(int currentUserId, int receiverId, DateTime? before, int? beforeId, int take);
        Task<Message?> GetLastMessageAsync(int userId1, int userId2, int currentUserId);
        Task<Dictionary<int, int>> GetUnreadCountsAsync(int receiverId);
        Task<List<Message>> GetStarredMessagesAsync();
        void Update(Message message);
        Task SaveChangesAsync();
        IQueryable<Message> Query();
    }
}