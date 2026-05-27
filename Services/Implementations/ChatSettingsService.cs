using Chat_App.Models;
using Chat_App.Repositories;
using Chat_App.Services.Dtos;
using Chat_App.Services.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
namespace Chat_App.Services.Implementations
{
    public class ChatSettingsService : IChatSettingsService
    {
        private readonly IChatBackgroundRepository _bgRepo;
        private readonly Cloudinary _cloudinary;
        public ChatSettingsService(IChatBackgroundRepository bgRepo, Cloudinary cloudinary)
        {
            _bgRepo = bgRepo;
            _cloudinary = cloudinary;
        }
        public async Task SetTimeColor(int userId, int peerId, string color)
        {
            var background = await _bgRepo.GetByUserAndPeerAsync(userId, peerId);
            if (background == null)
            {
                await _bgRepo.AddAsync(new ChatBackground
                {
                    UserId = userId,
                    PeerId = peerId,
                    MessageTimeColor = color,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                background.MessageTimeColor = color;
                background.UpdatedAt = DateTime.UtcNow;
                await _bgRepo.SaveChangesAsync();
            }
        }
        public async Task<BackgroundResultDto> SetBackground(int userId, int peerId, IFormFile file, string? backgroundFit)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"background/userBg_{userId}_{peerId}",
                Overwrite = true,
                Transformation = new Transformation().Width(1200)
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);
            var imageUrl = uploadResult.SecureUrl.ToString();
            var fit = backgroundFit ?? "cover";
            var existing = await _bgRepo.GetByUserAndPeerAsync(userId, peerId);
            if (existing != null)
            {
                existing.BackgroundImage = imageUrl;
                existing.BackgroundType = "image";
                existing.BackgroundFit = fit;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                await _bgRepo.AddAsync(new ChatBackground
                {
                    UserId = userId,
                    PeerId = peerId,
                    BackgroundImage = imageUrl,
                    BackgroundType = "image",
                    BackgroundFit = fit,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            await _bgRepo.SaveChangesAsync();
            return new BackgroundResultDto { BackgroundImage = imageUrl, BackgroundType = "image", BackgroundFit = fit };
        }
        public async Task<BackgroundResultDto> GetBackground(int userId, int peerId)
        {
            var bg = await _bgRepo.GetByUserAndPeerAsync(userId, peerId);
            if (bg == null)
                return new BackgroundResultDto { BackgroundImage = "", BackgroundFit = "cover", MessageTimeColor = "" };
            return new BackgroundResultDto
            {
                BackgroundImage = bg.BackgroundImage,
                BackgroundType = bg.BackgroundType,
                BackgroundFit = bg.BackgroundFit,
                MessageTimeColor = bg.MessageTimeColor
            };
        }
        public async Task RemoveBackground(int userId, int peerId)
        {
            var bg = await _bgRepo.GetByUserAndPeerAsync(userId, peerId);
            if (bg != null)
            {
                _bgRepo.Remove(bg);
                await _bgRepo.SaveChangesAsync();
            }
        }
        public async Task UpdateBackgroundFit(int userId, int peerId, string fit)
        {
            var bg = await _bgRepo.GetByUserAndPeerAsync(userId, peerId);
            if (bg != null)
            {
                bg.BackgroundFit = fit;
                bg.UpdatedAt = DateTime.UtcNow;
                await _bgRepo.SaveChangesAsync();
            }
        }
    }
}