namespace Chat_App.Models
{
    public class SavePushSubscriptionDto
    {
        public string Endpoint { get; set; }
        public string P256DH { get; set; }
        public string Auth { get; set; }
    }
}
