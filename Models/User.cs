using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat_App.Models
{
    public class UsersList
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public string username { get; set; }
        public string Name { get; set; }
        public string? NickName { get; set; }
        public string Gender { get; set; }

        public string? email { get; set; }

        public string password { get; set; }

        public DateTime? LastSeen { get; set; }

        public bool IsOnline { get; set; }

        public string? BlockedUsers { get; set; }

        public string? FileName { get; set; }

        public string? FileType { get; set; }

        public string? ProfileImagePath { get; set; }

        public string? HiddenPassword { get; set; }

        public int? Otp { get; set; }
        public string? Role { get; set; }

    }
}
