using Chat_App.Models;
using Chat_App.Repositories;
using Chat_App.Services.Interfaces;
namespace Chat_App.Services.Implementations
{
    public class PushService : IPushService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPushSubscriptionRepository _pushRepo;
        private readonly WebPushService _webPush;
        public PushService(IUserRepository userRepo, IPushSubscriptionRepository pushRepo, WebPushService webPush)
        {
            _userRepo = userRepo;
            _pushRepo = pushRepo;
            _webPush = webPush;
        }
        public async Task SaveSubscription(int userId, SavePushSubscriptionDto dto)
        {
            var existing = await _pushRepo.GetByUserIdAndEndpointAsync(userId, dto.Endpoint);
            if (existing != null)
            {
                existing.P256DH = dto.P256DH;
                existing.Auth = dto.Auth;
            }
            else
            {
                await _pushRepo.AddAsync(new PushSubscription
                {
                    UserId = userId,
                    Endpoint = dto.Endpoint,
                    P256DH = dto.P256DH,
                    Auth = dto.Auth
                });
            }
            await _pushRepo.SaveChangesAsync();
        }
        public async Task SendTestNotification(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            var title = user?.username ?? "Test";
            await _webPush.SendNotification(userId, title, "This is a test push notification from the server", "/chatapp.png", "/Chat/Index");
        }
    }
}