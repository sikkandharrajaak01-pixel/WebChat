namespace Chat_App.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyMessageDeletedForEveryone(int messageId, int senderId, int receiverId);
        Task NotifyMessageRestored(int messageId, int senderId, string? text, string? fileType, string? fileName, DateTime sentAt, bool isDelivered, bool isRead, int receiverId);
        Task NotifyGroupMessageDeleted(int messageId, int groupId, List<int> memberIds);
        Task NotifyGroupMessageRestored(int messageId, int groupId, int senderId, string? text, string? fileType, string? fileName, DateTime sentAt, double? duration, List<int> memberIds);
        Task NotifyFriendRequestReceived(int receiverId, int senderId);
        Task NotifyFriendRequestAccepted(int senderId, int accepterId);
        Task InvalidateMessageCache(int user1, int user2);
        Task InvalidateGroupMessageCache(int groupId);
    }
}
