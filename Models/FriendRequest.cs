using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat_App.Models
{
    public class FriendRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime SentAt { get; set; } = DateTime.Now;
        public DateTime? RespondedAt { get; set; }
        public string? Bond { get; set; }
    }
}
