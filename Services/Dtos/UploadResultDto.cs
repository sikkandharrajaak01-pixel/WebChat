namespace Chat_App.Services.Dtos
{
    public class UploadResultDto
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public double? Duration { get; set; }
    }

    public class GroupCreateResult
    {
        public bool Success { get; set; }
    }

    public class BackgroundResultDto
    {
        public string? BackgroundImage { get; set; }
        public string? BackgroundType { get; set; }
        public string? BackgroundFit { get; set; }
        public string? MessageTimeColor { get; set; }
    }
}
