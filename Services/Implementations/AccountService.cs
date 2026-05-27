using Chat_App.Models;
using Chat_App.Repositories;
using Chat_App.Services.Dtos;
using Chat_App.Services.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepo;
        private readonly IGroupMessageRepository _groupMsgRepo;
        private readonly IPushSubscriptionRepository _pushRepo;
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public AccountService(
            IUserRepository userRepo,
            IGroupMessageRepository groupMsgRepo,
            IPushSubscriptionRepository pushRepo,
            Cloudinary cloudinary,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _userRepo = userRepo;
            _groupMsgRepo = groupMsgRepo;
            _pushRepo = pushRepo;
            _cloudinary = cloudinary;
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<bool> EmailExists(string email)
            => await _userRepo.ExistsByEmailAsync(email);
        public async Task<bool> UsernameExists(string username)
            => await _userRepo.ExistsByUsernameAsync(username);
        public async Task<UsersList?> GetUserByEmail(string email)
            => await _userRepo.GetByEmailAsync(email);
        public async Task<UsersList?> GetUserByUsernameOrEmail(string username, string password)
            => await _userRepo.GetByUsernameOrEmailAsync(username, password);
        public async Task<UsersList?> GetUserById(int userId)
            => await _userRepo.GetByIdAsync(userId);
        public async Task CreateUser(UsersList user)
            => await _userRepo.AddAsync(user);
        public async Task ClearPushSubscriptions(int userId)
            => await _pushRepo.DeleteByUserIdAsync(userId);
        public async Task<(string? ImageUrl, string? FileName, string? FileType)> UploadProfilePhoto(IFormFile photo)
        {
            using var stream = photo.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(photo.FileName, stream),
                PublicId = $"profiles/{Guid.NewGuid()}",
                Overwrite = true,
                Transformation = new Transformation().Width(400).Height(400).Crop("fill").Gravity("face")
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
                return (null, null, null);
            return (uploadResult.SecureUrl.ToString(), photo.FileName, photo.ContentType);
        }
        public async Task<string?> UpdateProfileImage(int userId, IFormFile profileImage)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return null;
            using var stream = profileImage.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(profileImage.FileName, stream),
                PublicId = $"profiles/user_{user.Id}",
                Overwrite = true,
                Transformation = new Transformation().Width(400).Height(400).Crop("fill").Gravity("face")
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);
            var imageUrl = uploadResult.SecureUrl.ToString();
            user.FileName = profileImage.FileName;
            user.FileType = profileImage.ContentType;
            user.ProfileImagePath = imageUrl;
            var messages = await _groupMsgRepo.Query()
                .Where(x => x.SenderId == user.Id)
                .ToListAsync();
            foreach (var message in messages)
                message.SenderProfileImage = imageUrl;
            await _userRepo.SaveChangesAsync();
            return imageUrl;
        }
        public async Task<CheckMailResultDto> CheckMail(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null)
                return new CheckMailResultDto { Exists = false };
            var otp = new Random().Next(100000, 999999).ToString();
            user.Otp = int.Parse(otp);
            await _userRepo.SaveChangesAsync();
            try
            {
                await _emailService.SendOtpEmailAsync(email, otp);
                return new CheckMailResultDto { Exists = true, OtpSent = true };
            }
            catch
            {
                return new CheckMailResultDto { Exists = true, OtpSent = false, Error = "Failed to send OTP email. Check SMTP settings." };
            }
        }
        public async Task<bool> VerifyOtp(string email, string otp)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
                return false;
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null || user.Otp == null)
                return false;
            return user.Otp.ToString() == otp;
        }
        public async Task<(bool Success, string? Error)> ResetPassword(string email, string newPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
                return (false, "Invalid request.");
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null)
                return (false, "User not found.");
            user.password = newPassword;
            user.Otp = null;
            await _userRepo.SaveChangesAsync();
            return (true, null);
        }
        public async Task<EmailValidationResultDto> ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new EmailValidationResultDto { Exists = false, Valid = false };
            var emailExists = await _userRepo.ExistsByEmailAsync(email);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    _configuration["SniffMail:ApiKey"]);
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(new { email }),
                System.Text.Encoding.UTF8,
                "application/json");
            var response = await client.PostAsync(
                "https://api.sniffmail.io/verify", content);
            var json = await response.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            bool valid = doc.RootElement.GetProperty("is_deliverable").GetBoolean();
            return new EmailValidationResultDto { Exists = emailExists, Valid = valid };
        }
    }
}