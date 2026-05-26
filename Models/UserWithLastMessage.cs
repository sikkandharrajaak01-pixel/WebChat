namespace Chat_App.Models
{
    public class UserWithLastMessage
    {
        public UsersList User { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public int UnreadCount { get; set; }
    }
}
