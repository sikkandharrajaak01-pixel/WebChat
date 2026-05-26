using Chat_App.Repositories;
using Chat_App.Services.Dtos;
using Chat_App.Services.Interfaces;
namespace Chat_App.Services.Implementations
{
    public class BlockService : IBlockService
    {
        private readonly IUserRepository _userRepo;
        public BlockService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task BlockUser(int currentUserId, int userId)
        {
            var user = await _userRepo.GetByIdAsync(currentUserId);
            if (user == null) return;
            var blockedList = (user.BlockedUsers ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!blockedList.Contains(userId.ToString()))
            {
                blockedList.Add(userId.ToString());
                user.BlockedUsers = string.Join(",", blockedList);
                await _userRepo.SaveChangesAsync();
            }
        }
        public async Task UnblockUser(int currentUserId, int userId)
        {
            var user = await _userRepo.GetByIdAsync(currentUserId);
            if (user == null) return;
            var blockedList = (user.BlockedUsers ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (blockedList.Remove(userId.ToString()))
            {
                user.BlockedUsers = blockedList.Count > 0 ? string.Join(",", blockedList) : null;
                await _userRepo.SaveChangesAsync();
            }
        }
        public async Task<UsernameCheckResult> CheckUsername(string? username)
        {
            if (string.IsNullOrEmpty(username))
                return new UsernameCheckResult { Exists = false };
            var exists = await _userRepo.ExistsByUsernameAsync(username);
            return new UsernameCheckResult { Exists = exists };
        }
    }
}