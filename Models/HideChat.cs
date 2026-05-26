using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat_App.Models
{
    public class HideChat
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HideId { get; set; }

        public int UserId { get; set; }

        public List<int>? HideUsers { get; set; }
    }
}
