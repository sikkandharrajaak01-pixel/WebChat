using Chat_App.Models;
namespace Chat_App.Repositories
{
    public interface IChatBackgroundRepository
    {
        Task<ChatBackground?> GetByUserAndPeerAsync(int userId, int peerId);
        Task AddAsync(ChatBackground background);
        void Remove(ChatBackground background);
        Task SaveChangesAsync();
    }
}