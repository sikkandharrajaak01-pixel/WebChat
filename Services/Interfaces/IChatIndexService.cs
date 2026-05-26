using Chat_App.Models;

namespace Chat_App.Services.Interfaces
{
    public interface IChatIndexService
    {
        Task<List<ChatConversationItem>> GetConversations(int currentUserId);
        Task<List<ChatConversationItem>> GetHiddenConversations(int currentUserId);
        Task<(List<UsersList> AllUsers, List<GroupCredentials> AllGroups)> GetHiddenViewExtras(int currentUserId, List<int> hiddenUserIds, List<int> hiddenGroupIds);
        Task<bool> VerifyHiddenAccess(int userId, string password, string? confirmPassword);
        bool IsHiddenAccessGranted(int userId, int sessionUserId);
        Task<List<int>> GetHiddenUserIds(int currentUserId);
        Task<List<int>> GetHiddenGroupIds(int currentUserId);
    }
}
