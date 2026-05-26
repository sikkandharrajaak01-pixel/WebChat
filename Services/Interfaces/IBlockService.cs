using Chat_App.Services.Dtos;

namespace Chat_App.Services.Interfaces
{
    public interface IBlockService
    {
        Task BlockUser(int currentUserId, int userId);
        Task UnblockUser(int currentUserId, int userId);
        Task<UsernameCheckResult> CheckUsername(string? username);
    }
}
