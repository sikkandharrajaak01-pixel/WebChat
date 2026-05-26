namespace Chat_App.Models
{
    public class Moment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string MediaUrl { get; set; }
        public string FileType { get; set; }   // "image" or "video"
        public string? Caption { get; set; }
        public string CloudinaryPublicId { get; set; } // needed to delete from Cloudinary
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }       // CreatedAt + 24h
        public ICollection<MomentView> Views { get; set; }

    }
}
