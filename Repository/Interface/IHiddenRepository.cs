using Chat_App.Models;
namespace Chat_App.Repositories
{
    public interface IHiddenRepository
    {
        Task<List<int>> GetHiddenUserIdsAsync(int userId);
        Task<List<int>> GetHiddenGroupIdsAsync(int userId);
        Task<HiddenChat?> GetHiddenChatAsync(int userId, int hiddenUserId);
        Task<HiddenGroup?> GetHiddenGroupAsync(int userId, int groupId);
        void RemoveHiddenChat(HiddenChat chat);
        void RemoveHiddenGroup(HiddenGroup group);
        Task AddHiddenChatAsync(HiddenChat chat);
        Task AddHiddenGroupAsync(HiddenGroup group);
        Task SaveChangesAsync();
    }
}