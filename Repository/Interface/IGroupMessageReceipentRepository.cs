using Chat_App.Models;
namespace Chat_App.Repositories
{
    public interface IGroupMessageRecipientRepository
    {
        Task<GroupMessageRecipient?> GetByMessageAndUserAsync(int messageId, int userId);
        Task<List<GroupMessageRecipient>> GetByMessageIdAsync(int messageId);
        Task<int> GetDeliveredCountAsync(int messageId);
        Task<int> GetReadCountAsync(int messageId);
        Task<Dictionary<int, int>> GetGroupUnreadCountsAsync(int userId, List<int> groupIds);
        Task AddAsync(GroupMessageRecipient recipient);
        Task SaveChangesAsync();
        IQueryable<GroupMessageRecipient> Query();
        Task<Dictionary<int, (int Delivered, int Read)>> GetCountsForMessagesAsync(List<int> messageIds);
    }
}