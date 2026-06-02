namespace Chat_App.Models
{
    public class GroupMessage
    {
        public int GroupMessageId { get; set; }
        public int GroupId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string? SenderProfileImage { get; set; }
        public string? Text { get; set; }
        public DateTime SentAt { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string? DeletedStatus { get; set; }
        public int? DeletedForUserId { get; set; }

        public bool? IsStared { get; set; } = false;
        public double? Duration { get; set; }

        public bool IsForwarded { get; set; } = false;
        public int? OriginalMessageId { get; set; }
    }
}

