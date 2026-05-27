using Chat_App.Models;
using Chat_App.Services.Dtos;
namespace Chat_App.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> EmailExists(string email);
        Task<bool> UsernameExists(string username);
        Task<UsersList?> GetUserByEmail(string email);
        Task<UsersList?> GetUserByUsernameOrEmail(string username, string password);
        Task<UsersList?> GetUserById(int userId);
        Task CreateUser(UsersList user);
        Task<(string? ImageUrl, string? FileName, string? FileType)> UploadProfilePhoto(IFormFile photo);
        Task<string?> UpdateProfileImage(int userId, IFormFile profileImage);
        Task<CheckMailResultDto> CheckMail(string email);
        Task<bool> VerifyOtp(string email, string otp);
        Task<(bool Success, string? Error)> ResetPassword(string email, string newPassword);
        Task<EmailValidationResultDto> ValidateEmail(string email);
        Task ClearPushSubscriptions(int userId);
    }
}