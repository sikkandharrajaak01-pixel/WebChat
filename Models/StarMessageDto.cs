namespace Chat_App.Models
{
   
        public class StarMessageDto
        {
            public string MessageType { get; set; } // Group or Personal

            public int Id { get; set; }

            public int SenderId { get; set; }

            public int? ReceiverId { get; set; }

            public int? GroupId { get; set; }

            public string? SenderName { get; set; }

            public string? SenderProfileImage { get; set; }

            public string? Text { get; set; }

            public string? FileType { get; set; }

            public string? FileName { get; set; }

            public DateTime SentAt { get; set; }
        }
   
}
