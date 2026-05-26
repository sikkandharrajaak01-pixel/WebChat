using Chat_App.Models;
using Microsoft.EntityFrameworkCore;
using WebPush;

namespace Chat_App.Services
{
    public class WebPushService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _vapidPublicKey;
        private readonly string _vapidPrivateKey;
        private const string VapidSubject = "mailto:chat-app@example.com";

        public WebPushService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _vapidPublicKey = configuration["Vapid:PublicKey"];
            _vapidPrivateKey = configuration["Vapid:PrivateKey"];
        }

        public string PublicKey => _vapidPublicKey;

        public async Task SendNotification(int userId, string title, string body, string icon, string dataUrl)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

            var subscriptions = await db.pushSubscription
                .Where(s => s.UserId == userId)
                .ToListAsync();

            foreach (var sub in subscriptions)
            {
                try
                {
                    var pushSubscription = new WebPush.PushSubscription
                    {
                        Endpoint = sub.Endpoint,
                        P256DH = sub.P256DH,
                        Auth = sub.Auth
                    };

                    var vapidDetails = new VapidDetails(VapidSubject, _vapidPublicKey, _vapidPrivateKey);

                    var payload = new
                    {
                        title,
                        body,
                        icon,
                        data = new { url = dataUrl }
                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(payload);

                    var client = new WebPushClient();
                    await client.SendNotificationAsync(pushSubscription, json, vapidDetails);
                }
                catch (WebPushException ex)
                {
                    Console.Error.WriteLine($"[WebPush] Failed for user {userId}: {ex.StatusCode} - {ex.Message}");

                    if (ex.StatusCode == System.Net.HttpStatusCode.Gone ||
                        ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        db.pushSubscription.Remove(sub);
                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[WebPush] Unexpected error for user {userId}: {ex.Message}");
                }
            }
        }
    }
}
