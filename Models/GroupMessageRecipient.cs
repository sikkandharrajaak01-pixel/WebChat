namespace Chat_App.Models
{
    public class GroupMessageRecipient
    {
        public int Id { get; set; }
        public int GroupMessageId { get; set; }
        public int UserId { get; set; }
        public bool IsDelivered { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
