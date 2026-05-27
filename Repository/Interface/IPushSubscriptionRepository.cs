using Chat_App.Models;
namespace Chat_App.Repositories
{
    public interface IPushSubscriptionRepository
    {
        Task<PushSubscription?> GetByUserIdAndEndpointAsync(int userId, string endpoint);
        Task AddAsync(PushSubscription subscription);
        Task SaveChangesAsync();
        Task DeleteByUserIdAsync(int userId);
    }
}