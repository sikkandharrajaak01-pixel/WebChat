using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat_App.Models
{
    public class GroupCredentials
    {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int GroupId { get; set; }
            public string GroupName { get; set; }

            public List<int> UserIds { get; set; }
            public string? FileName { get; set; }

            public string? FileType { get; set; }

            public string? ProfileImagePath { get; set; }

            public List<int>? AdminIds { get; set; }
        }
    }

