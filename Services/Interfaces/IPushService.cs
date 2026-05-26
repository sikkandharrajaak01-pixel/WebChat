using Chat_App.Models;

namespace Chat_App.Services.Interfaces
{
    public interface IPushService
    {
        Task SaveSubscription(int userId, SavePushSubscriptionDto dto);
        Task SendTestNotification(int userId);
    }
}
