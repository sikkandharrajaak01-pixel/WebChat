using Chat_App.Models;
using Chat_App.Repositories;
using Chat_App.Services.Interfaces;
namespace Chat_App.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepo;
        public ProfileService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<UsersList?> GetProfile(int userId)
            => await _userRepo.GetByIdAsync(userId);
        public async Task UpdateProfile(int userId, string? username, string? name, string? nickName)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return;
            if (!string.IsNullOrWhiteSpace(username))
                user.username = username;
            if (!string.IsNullOrWhiteSpace(name))
                user.Name = name;
            user.NickName = nickName;
            await _userRepo.SaveChangesAsync();
        }
    }
}