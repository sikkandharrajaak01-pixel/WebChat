using Chat_App.Models;
using Chat_App.Repositories;
using Chat_App.Services.Dtos;
using Chat_App.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Services.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IGroupMessageRepository _groupMsgRepo;
        private readonly IGroupMessageRecipientRepository _groupMsgRecipientRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IUserRepository _userRepo;
        private readonly INotificationService _notification;
        public MessageService(
            IMessageRepository messageRepo,
            IGroupMessageRepository groupMsgRepo,
            IGroupMessageRecipientRepository groupMsgRecipientRepo,
            IGroupRepository groupRepo,
            IUserRepository userRepo,
            INotificationService notification)
        {
            _messageRepo = messageRepo;
            _groupMsgRepo = groupMsgRepo;
            _groupMsgRecipientRepo = groupMsgRecipientRepo;
            _groupRepo = groupRepo;
            _userRepo = userRepo;
            _notification = notification;
        }
        public async Task<MessageListResult> GetMessages(int currentUserId, int receiverId, DateTime? before, int? beforeId, int take = 50)
        {
            var messages = await _messageRepo.GetMessagesAsync(currentUserId, receiverId, before, beforeId, take + 1);
            var hasMore = messages.Count > take;
            if (hasMore) messages.RemoveAt(messages.Count - 1);
            messages.Reverse();
            var result = messages.Select(m =>
            {
                if (m.DeletedStatus == "EveryOne")
                    return new MessageDto { MessageId = m.MessageId, SenderId = m.SenderId, Text = "This message was deleted", SentAt = m.SentAt, IsDeleted = true, IsStared = m.IsStared };
                if (m.DeletedStatus == "Forme" && m.DeletedForUserId == currentUserId)
                    return null;
                return new MessageDto
                {
                    MessageId = m.MessageId,
                    SenderId = m.SenderId,
                    Text = m.Text,
                    SentAt = m.SentAt,
                    FileType = m.FileType,
                    FileName = m.FileName,
                    IsDelivered = m.IsDelivered,
                    IsRead = m.IsRead,
                    IsStared = m.IsStared,
                    IsDeleted = false
                };
            }).Where(m => m != null).Cast<MessageDto>().ToList();
            return new MessageListResult { Messages = result, HasMore = hasMore };
        }
        public async Task<GroupMessageListResult> GetGroupMessages(int currentUserId, int groupId, DateTime? before, int? beforeId, int take = 50)
        {
            var messages = await _groupMsgRepo.GetGroupMessagesAsync(groupId, currentUserId, before, beforeId, take + 1);
            var hasMore = messages.Count > take;
            if (hasMore) messages.RemoveAt(messages.Count - 1);
            messages.Reverse();
            var group = await _groupRepo.GetByIdAsync(groupId);
            var totalMembers = group?.UserIds?.Count ?? 0;
            var totalRecipients = totalMembers > 0 ? totalMembers - 1 : 0;
            var result = new List<GroupMessageDto>();
            foreach (var m in messages)
            {
                int deliveredCount = 0, readCount = 0;
                if (m.SenderId == currentUserId && totalRecipients > 0)
                {
                    deliveredCount = await _groupMsgRecipientRepo.GetDeliveredCountAsync(m.GroupMessageId);
                    readCount = await _groupMsgRecipientRepo.GetReadCountAsync(m.GroupMessageId);
                }
                result.Add(new GroupMessageDto
                {
                    GroupMessageId = m.GroupMessageId,
                    GroupId = m.GroupId,
                    SenderId = m.SenderId,
                    SenderName = m.SenderName,
                    SenderProfileImage = m.SenderProfileImage,
                    Text = m.DeletedStatus == "EveryOne" ? "This message was deleted" : m.Text,
                    SentAt = m.SentAt,
                    FileType = m.DeletedStatus == "EveryOne" ? null : m.FileType,
                    FileName = m.DeletedStatus == "EveryOne" ? null : m.FileName,
                    IsMine = m.SenderId == currentUserId,
                    IsDeleted = m.DeletedStatus == "EveryOne",
                    DeliveredCount = deliveredCount,
                    ReadCount = readCount,
                    TotalRecipients = totalRecipients,
                    IsStared = m.IsStared ?? false
                });
            }
            return new GroupMessageListResult { Messages = result, HasMore = hasMore };
        }
        public async Task<GroupMessageStatusResult> GetGroupMessageStatus(int currentUserId, int messageId)
        {
            var msg = await _groupMsgRepo.GetByIdAsync(messageId);
            if (msg == null) return new GroupMessageStatusResult();
            var group = await _groupRepo.GetByIdAsync(msg.GroupId);
            if (group?.UserIds == null || !group.UserIds.Contains(currentUserId))
                return new GroupMessageStatusResult();
            var allMembers = await _userRepo.Query()
                .Where(u => group.UserIds.Contains(u.Id) && u.Id != msg.SenderId)
                .ToListAsync();
            var recipients = await _groupMsgRecipientRepo.GetByMessageIdAsync(messageId);
            var recipientDict = recipients.ToDictionary(r => r.UserId);
            var result = new GroupMessageStatusResult();
            foreach (var member in allMembers)
            {
                recipientDict.TryGetValue(member.Id, out var recipient);
               
               var dto = new RecipientStatusDto
               {
                   UserId = member.Id,
                   Username = member.username,
                   ProfileImage = member.ProfileImagePath,
                   ReadAt = recipient?.ReadAt,
                   DeliveredAt = recipient?.DeliveredAt
               };
                if (recipient != null && recipient.IsRead)
                    result.Read.Add(dto);
                else if (recipient != null && recipient.IsDelivered)
                    result.Delivered.Add(dto);
                else
                    result.Pending.Add(dto);
            }
            return result;
        }
        public async Task<List<StarMessageDto>> GetStarredMessages()
        {
            var groupMessages = (await _groupMsgRepo.GetStarredGroupMessagesAsync())
                .Select(x => new StarMessageDto
                {
                    MessageType = "Group",
                    Id = x.GroupMessageId,
                    GroupId = x.GroupId,
                    SenderId = x.SenderId,
                    SenderName = x.SenderName,
                    SenderProfileImage = x.SenderProfileImage,
                    Text = x.Text,
                    FileType = x.FileType,
                    FileName = x.FileName,
                    SentAt = x.SentAt
                }).ToList();
            var chatMessages = (await _messageRepo.GetStarredMessagesAsync())
                .Select(x => new StarMessageDto
                {
                    MessageType = "Personal",
                    Id = x.MessageId,
                    SenderId = x.SenderId,
                    ReceiverId = x.ReceiverId,
                    Text = x.Text,
                    FileType = x.FileType,
                    FileName = x.FileName,
                    SentAt = x.SentAt
                }).ToList();
            return groupMessages.Concat(chatMessages).OrderByDescending(x => x.SentAt).ToList();
        }
        public async Task StarMessage(int messageId)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message != null)
            {
                message.IsStared = true;
                await _messageRepo.SaveChangesAsync();
            }
        }
        public async Task UnstarMessage(int messageId)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message != null)
            {
                message.IsStared = false;
                await _messageRepo.SaveChangesAsync();
            }
        }
        public async Task StarGroupMessage(int messageId)
        {
            var message = await _groupMsgRepo.GetByIdAsync(messageId);
            if (message != null)
            {
                message.IsStared = true;
                await _groupMsgRepo.SaveChangesAsync();
            }
        }
        public async Task UnstarGroupMessage(int messageId)
        {
            var message = await _groupMsgRepo.GetByIdAsync(messageId);
            if (message != null)
            {
                message.IsStared = false;
                await _groupMsgRepo.SaveChangesAsync();
            }
        }
        public async Task RemoveStar(int messageId, string messageType)
        {
            if (messageType == "Personal")
            {
                var message = await _messageRepo.GetByIdAsync(messageId);
                if (message != null)
                {
                    message.IsStared = false;
                    await _messageRepo.SaveChangesAsync();
                }
            }
            else if (messageType == "Group")
            {
                var message = await _groupMsgRepo.GetByIdAsync(messageId);
                if (message != null)
                {
                    message.IsStared = false;
                    await _groupMsgRepo.SaveChangesAsync();
                }
            }
        }
        public async Task DeleteForEveryone(int messageId, int currentUserId)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null || message.SenderId != currentUserId) return;
            message.DeletedStatus = "EveryOne";
            message.DeletedForUserId = null;
            _messageRepo.Update(message);
            await _messageRepo.SaveChangesAsync();
            await _notification.InvalidateMessageCache(message.SenderId, message.ReceiverId);
            await _notification.NotifyMessageDeletedForEveryone(messageId, message.SenderId, message.ReceiverId);
        }
        public async Task<DeleteResult?> DeleteForMe(int messageId, int currentUserId)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null) return null;
            message.DeletedStatus = "Forme";
            message.DeletedForUserId = currentUserId;
            _messageRepo.Update(message);
            await _messageRepo.SaveChangesAsync();
            await _notification.InvalidateMessageCache(message.SenderId, message.ReceiverId);
            return new DeleteResult { Success = true };
        }
        public async Task<DeleteResult?> UndoDelete(int messageId, int currentUserId)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null || message.SenderId != currentUserId) return null;
            message.DeletedStatus = null;
            message.DeletedForUserId = null;
            _messageRepo.Update(message);
            await _messageRepo.SaveChangesAsync();
            await _notification.InvalidateMessageCache(message.SenderId, message.ReceiverId);
            await _notification.NotifyMessageRestored(messageId, message.SenderId, message.Text,
                message.FileType, message.FileName, message.SentAt, message.IsDelivered, message.IsRead, message.ReceiverId);
            return new DeleteResult
            {
                Success = true,
                MessageId = message.MessageId,
                Text = message.Text,
                SenderId = message.SenderId,
                FileType = message.FileType,
                FileName = message.FileName,
                SentAt = message.SentAt,
                IsDelivered = message.IsDelivered,
                IsRead = message.IsRead
            };
        }
        public async Task DeleteGroupMessageForEveryone(int messageId, int currentUserId)
        {
            var message = await _groupMsgRepo.GetByIdAsync(messageId);
            if (message == null || message.SenderId != currentUserId) return;
            message.DeletedStatus = "EveryOne";
            message.DeletedForUserId = null;
            _groupMsgRepo.Update(message);
            await _groupMsgRepo.SaveChangesAsync();
            await _notification.InvalidateGroupMessageCache(message.GroupId);
            var group = await _groupRepo.GetByIdAsync(message.GroupId);
            if (group?.UserIds != null)
                await _notification.NotifyGroupMessageDeleted(messageId, message.GroupId, group.UserIds);
        }
        public async Task DeleteGroupMessageForMe(int messageId, int currentUserId)
        {
            var message = await _groupMsgRepo.GetByIdAsync(messageId);
            if (message == null) return;
            message.DeletedStatus = "Forme";
            message.DeletedForUserId = currentUserId;
            _groupMsgRepo.Update(message);
            await _groupMsgRepo.SaveChangesAsync();
            await _notification.InvalidateGroupMessageCache(message.GroupId);
        }
        public async Task<DeleteResult?> UndoGroupDelete(int messageId, int currentUserId)
        {
            var message = await _groupMsgRepo.GetByIdAsync(messageId);
            if (message == null || message.SenderId != currentUserId) return null;
            message.DeletedStatus = null;
            message.DeletedForUserId = null;
            _groupMsgRepo.Update(message);
            await _groupMsgRepo.SaveChangesAsync();
            await _notification.InvalidateGroupMessageCache(message.GroupId);
            var group = await _groupRepo.GetByIdAsync(message.GroupId);
            if (group?.UserIds != null)
                await _notification.NotifyGroupMessageRestored(messageId, message.GroupId, message.SenderId,
                    message.Text, message.FileType, message.FileName, message.SentAt, message.Duration, group.UserIds);
            return new DeleteResult
            {
                Success = true,
                MessageId = message.GroupMessageId,
                Text = message.Text,
                SenderId = message.SenderId,
                SenderName = message.SenderName,
                FileType = message.FileType,
                FileName = message.FileName,
                SentAt = message.SentAt
            };
        }
    }
}