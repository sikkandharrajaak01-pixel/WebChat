using Chat_App.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
namespace Chat_App.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IDatabase _database;
        public NotificationService(IHubContext<ChatHub> hubContext, IConnectionMultiplexer redis)
        {
            _hubContext = hubContext;
            _database = redis.GetDatabase();
        }
        public async Task NotifyMessageDeletedForEveryone(int messageId, int senderId, int receiverId)
        {
            var tasks = new List<Task>();
            foreach (var connId in ConnectionManager.GetConnections(receiverId))
                tasks.Add(_hubContext.Clients.Client(connId).SendAsync("MessageDeletedForEveryone", messageId));
            foreach (var connId in ConnectionManager.GetConnections(senderId))
                tasks.Add(_hubContext.Clients.Client(connId).SendAsync("MessageDeletedForEveryone", messageId));
            await Task.WhenAll(tasks);
        }
        public async Task NotifyMessageRestored(int messageId, int senderId, string? text, string? fileType, string? fileName, DateTime sentAt, bool isDelivered, bool isRead, int receiverId)
        {
            var tasks = new List<Task>();
            foreach (var connId in ConnectionManager.GetConnections(receiverId))
                tasks.Add(_hubContext.Clients.Client(connId).SendAsync("MessageRestored", messageId, senderId, text, fileType, fileName, sentAt, isDelivered, isRead));
            foreach (var connId in ConnectionManager.GetConnections(senderId))
                tasks.Add(_hubContext.Clients.Client(connId).SendAsync("MessageRestored", messageId, senderId, text, fileType, fileName, sentAt, isDelivered, isRead));
            await Task.WhenAll(tasks);
        }
        public async Task NotifyGroupMessageDeleted(int messageId, int groupId, List<int> memberIds)
        {
            var tasks = new List<Task>();
            foreach (var memberId in memberIds)
            {
                foreach (var connId in ConnectionManager.GetConnections(memberId))
                    tasks.Add(_hubContext.Clients.Client(connId).SendAsync("GroupMessageDeleted", messageId, groupId));
            }
            await Task.WhenAll(tasks);
        }
        public async Task NotifyGroupMessageRestored(int messageId, int groupId, int senderId, string? text, string? fileType, string? fileName, DateTime sentAt, double? duration, List<int> memberIds)
        {
            var tasks = new List<Task>();
            foreach (var memberId in memberIds)
            {
                foreach (var connId in ConnectionManager.GetConnections(memberId))
                    tasks.Add(_hubContext.Clients.Client(connId).SendAsync("GroupMessageRestored", messageId, groupId, senderId, text, fileType, fileName, sentAt, duration));
            }
            await Task.WhenAll(tasks);
        }
        public async Task NotifyFriendRequestReceived(int receiverId, int senderId)
        {
            foreach (var connId in ConnectionManager.GetConnections(receiverId))
            {
                try
                {
                    await _hubContext.Clients.Client(connId).SendAsync("FriendRequestReceived", senderId);
                }
                catch { }
            }
        }
        public async Task NotifyFriendRequestAccepted(int senderId, int accepterId)
        {
            foreach (var connId in ConnectionManager.GetConnections(senderId))
            {
                try
                {
                    await _hubContext.Clients.Client(connId).SendAsync("FriendRequestAccepted", accepterId);
                }
                catch { }
            }
        }
        public async Task InvalidateMessageCache(int user1, int user2)
        {
            var key1 = $"messages:{user1}:{user2}";
            var key2 = $"messages:{user2}:{user1}";
            await _database.KeyDeleteAsync(key1);
            await _database.KeyDeleteAsync(key2);
        }
        public async Task InvalidateGroupMessageCache(int groupId)
        {
            await _database.KeyDeleteAsync($"messages:group:{groupId}");

        }
    }
}