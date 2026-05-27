using Chat_App.Models;
using Chat_App.Repositories;
using Chat_App.Services.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
namespace Chat_App.Services.Implementations
{
    public class MomentService : IMomentService
    {
        private readonly IMomentRepository _momentRepo;
        private readonly IFriendRequestRepository _friendRequestRepo;
        private readonly Cloudinary _cloudinary;
        public MomentService(IMomentRepository momentRepo, IFriendRequestRepository friendRequestRepo, Cloudinary cloudinary)
        {
            _momentRepo = momentRepo;
            _friendRequestRepo = friendRequestRepo;
            _cloudinary = cloudinary;
        }
        public async Task<Moment> UploadMoment(int userId, IFormFile file, string fileType, string? caption)
        {
            using var stream = file.OpenReadStream();
            var publicId = $"moments/{userId}/{Guid.NewGuid()}";
            string mediaUrl;
            switch (fileType)
            {
                case "video":
                    var videoParams = new VideoUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = publicId
                    };
                    var videoResult = await _cloudinary.UploadAsync(videoParams);
                    if (videoResult.Error != null)
                        throw new Exception(videoResult.Error.Message);
                    mediaUrl = videoResult.SecureUrl.ToString();
                    break;
                default:
                    var imageParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = publicId
                    };
                    var imageResult = await _cloudinary.UploadAsync(imageParams);
                    if (imageResult.Error != null)
                        throw new Exception(imageResult.Error.Message);
                    mediaUrl = imageResult.SecureUrl.ToString();
                    break;
            }
            var now = DateTime.UtcNow;
            var moment = new Moment
            {
                UserId = userId,
                MediaUrl = mediaUrl,
                FileType = fileType,
                Caption = caption,
                CloudinaryPublicId = publicId,
                CreatedAt = now,
                ExpiresAt = now.AddHours(24),
                Views = new List<MomentView>()
            };
            await _momentRepo.AddMomentAsync(moment);
            return moment;
        }
        public async Task<List<Moment>> GetActiveMoments(int currentUserId)
        {
            var friendIds = await _friendRequestRepo.GetAcceptedFriendIdsAsync(currentUserId);
            friendIds.Add(currentUserId);
            return await _momentRepo.GetActiveMomentsAsync(friendIds);
        }
        public async Task<List<Moment>> GetMyMoments(int userId)
            => await _momentRepo.GetMyMomentsAsync(userId);
        public async Task MarkAsViewed(int momentId, int viewerId)
        {
            var existing = await _momentRepo.GetExistingViewAsync(momentId, viewerId);
            if (existing != null) return;
            await _momentRepo.AddViewAsync(new MomentView
            {
                MomentId = momentId,
                ViewedByUserId = viewerId,
                ViewedAt = DateTime.UtcNow
            });
        }
        public async Task<bool> DeleteMoment(int momentId, int userId)
        {
            var moment = await _momentRepo.GetMomentByIdAsync(momentId);
            if (moment == null || moment.UserId != userId)
                return false;
            try
            {
                await _cloudinary.DestroyAsync(new DeletionParams(moment.CloudinaryPublicId));
            }
            catch
            { }
            _momentRepo.RemoveMoment(moment);
            await _momentRepo.SaveChangesAsync();
            return true;
            
        }
        public async Task<int> GetViewCount(int momentId)
            => await _momentRepo.GetViewCountAsync(momentId);
        public async Task DeleteExpiredMoments()
        {
            var expired = await _momentRepo.GetExpiredMomentsAsync();
            if (expired.Count == 0) return;
            foreach (var moment in expired)
            {
                try
                {
                    await _cloudinary.DestroyAsync(new DeletionParams(moment.CloudinaryPublicId));
                }
                catch { }
                _momentRepo.RemoveMoment(moment);
            }
            await _momentRepo.SaveChangesAsync();
        }
    }
}