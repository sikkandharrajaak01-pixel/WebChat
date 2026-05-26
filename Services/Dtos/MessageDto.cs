namespace Chat_App.Services.Dtos
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string? Text { get; set; }
        public DateTime SentAt { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public bool IsDelivered { get; set; }
        public bool IsRead { get; set; }
        public bool IsStared { get; set; }
        public bool IsDeleted { get; set; }
        public double? Duration { get; set; }
    }

    public class MessageListResult
    {
        public List<MessageDto> Messages { get; set; } = new();
        public bool HasMore { get; set; }
    }

    public class GroupMessageDto
    {
        public int GroupMessageId { get; set; }
        public int GroupId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? SenderProfileImage { get; set; }
        public string? Text { get; set; }
        public DateTime SentAt { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public bool IsMine { get; set; }
        public bool IsDeleted { get; set; }
        public int DeliveredCount { get; set; }
        public int ReadCount { get; set; }
        public int TotalRecipients { get; set; }
        public bool IsStared { get; set; }
        public double? Duration { get; set; }
    }

    public class GroupMessageListResult
    {
        public List<GroupMessageDto> Messages { get; set; } = new();
        public bool HasMore { get; set; }
    }

    public class DeleteResult
    {
        public bool Success { get; set; }
        public int MessageId { get; set; }
        public string? Text { get; set; }
        public int SenderId { get; set; }
        public string? SenderName { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public DateTime? SentAt { get; set; }
        public bool IsDelivered { get; set; }
        public bool IsRead { get; set; }
    }

    public class GroupMessageStatusResult
    {
        public List<RecipientStatusDto> Delivered { get; set; } = new();
        public List<RecipientStatusDto> Read { get; set; } = new();
        public List<RecipientStatusDto> Pending { get; set; } = new();
    }

    public class RecipientStatusDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
