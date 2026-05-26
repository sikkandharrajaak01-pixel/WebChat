namespace Chat_App.Services.Dtos
{
    public class FriendDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
    }

    public class PendingFriendRequestDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? SenderProfileImage { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class SentFriendRequestDto
    {
        public int Id { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string? ReceiverProfileImage { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class FriendshipStatusDto
    {
        public string Status { get; set; } = "None";
        public int? RequestId { get; set; }
        public bool IsSender { get; set; }
    }

    public class UserForFriendRequestDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public bool IsOnline { get; set; }
        public FriendshipStatusDto? FriendshipStatus { get; set; }
    }

    public class UserSearchResultDto
    {
        public List<UserForFriendRequestDto> Users { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class FriendRequestResult
    {
        public bool Success { get; set; }
    }

    public class UsernameCheckResult
    {
        public bool Exists { get; set; }
    }
}
