namespace Chat_App.Services.Dtos
{
    public class CheckMailResultDto
    {
        public bool Exists { get; set; }
        public bool OtpSent { get; set; }
        public string? Error { get; set; }
    }
}
