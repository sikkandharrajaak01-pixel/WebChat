using Chat_App.Models;
namespace Chat_App.Repositories
{
    public interface IGroupMessageRepository
    {
        Task<GroupMessage?> GetByIdAsync(int messageId);
        Task<List<GroupMessage>> GetGroupMessagesAsync(int groupId, int currentUserId, DateTime? before, int? beforeId, int take);
        Task<GroupMessage?> GetLastMessageAsync(int groupId, int currentUserId);
        Task<List<GroupMessage>> GetStarredGroupMessagesAsync();
        void Update(GroupMessage message);
        Task SaveChangesAsync();
        IQueryable<GroupMessage> Query();
       
    }
}