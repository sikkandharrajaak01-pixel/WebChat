using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat_App.Models
{
    public class ChatBackground
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PeerId { get; set; }
        public string? BackgroundImage { get; set; }
        public string? BackgroundType { get; set; } // "image" | "solid" | "gradient"
        public string? BackgroundFit { get; set; } = "cover"; // "cover" | "contain" | "repeat"

        public string? MessageTimeColor { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
