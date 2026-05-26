namespace Chat_App.Models
{
    public class MomentView
    {
        public int Id { get; set; }
        public int MomentId { get; set; }
        public int ViewedByUserId { get; set; }
        public DateTime ViewedAt { get; set; }
        public Moment Moment { get; set; }
    }
}

