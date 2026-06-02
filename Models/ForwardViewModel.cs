namespace Chat_App.Models
{
    public class ForwardMessageDto
    {
        public int MessageId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public string? FileName { get; set; }
    }

    public class ForwardRecipientDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
    }

    public class ForwardGroupDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public List<int> UserIds { get; set; } = new();
    }

    public class ForwardViewModel
    {
        public ForwardMessageDto Message { get; set; } = new();
        public List<ForwardRecipientDto> Users { get; set; } = new();
        public List<ForwardGroupDto> Groups { get; set; } = new();
    }

    public class ForwardRequest
    {
        public int MessageId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public List<RecipientSelection> Recipients { get; set; } = new();
    }

    public class RecipientSelection
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // "user" or "group"
    }

    public class ForwardResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
