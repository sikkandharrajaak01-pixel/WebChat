using Chat_App.Services.Dtos;

namespace Chat_App.Services.Interfaces
{
    public interface IChatSettingsService
    {
        Task SetTimeColor(int userId, int peerId, string color);
        Task<BackgroundResultDto> SetBackground(int userId, int peerId, IFormFile file, string? backgroundFit);
        Task<BackgroundResultDto> GetBackground(int userId, int peerId);
        Task RemoveBackground(int userId, int peerId);
        Task UpdateBackgroundFit(int userId, int peerId, string fit);
    }
}
