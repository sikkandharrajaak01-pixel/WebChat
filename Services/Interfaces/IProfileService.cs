using Chat_App.Models;

namespace Chat_App.Services.Interfaces
{
    public interface IProfileService
    {
        Task<UsersList?> GetProfile(int userId);
        Task UpdateProfile(int userId, string? username, string? name, string? nickName);
    }
}
