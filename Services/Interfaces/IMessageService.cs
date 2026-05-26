using Chat_App.Models;
using Chat_App.Services.Dtos;

namespace Chat_App.Services.Interfaces
{
    public interface IMessageService
    {
        Task<MessageListResult> GetMessages(int currentUserId, int receiverId, DateTime? before, int? beforeId, int take = 50);
        Task<GroupMessageListResult> GetGroupMessages(int currentUserId, int groupId, DateTime? before, int? beforeId, int take = 50);
        Task<GroupMessageStatusResult> GetGroupMessageStatus(int currentUserId, int messageId);
        Task<List<StarMessageDto>> GetStarredMessages();
        Task StarMessage(int messageId);
        Task UnstarMessage(int messageId);
        Task RemoveStar(int messageId, string messageType);
        Task StarGroupMessage(int messageId);
        Task UnstarGroupMessage(int messageId);
        Task DeleteForEveryone(int messageId, int currentUserId);
        Task<DeleteResult?> DeleteForMe(int messageId, int currentUserId);
        Task<DeleteResult?> UndoDelete(int messageId, int currentUserId);
        Task DeleteGroupMessageForEveryone(int messageId, int currentUserId);
        Task DeleteGroupMessageForMe(int messageId, int currentUserId);
        Task<DeleteResult?> UndoGroupDelete(int messageId, int currentUserId);
    }
}
