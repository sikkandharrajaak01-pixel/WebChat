using Chat_App.Models;
namespace Chat_App.Repositories
{
    public interface IUserRepository
    {
        Task<UsersList?> GetByIdAsync(int id);
        Task<UsersList?> GetByEmailAsync(string email);
        Task<UsersList?> GetByUsernameOrEmailAsync(string username, string password);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username);
        Task AddAsync(UsersList user);
        Task SaveChangesAsync();
        Task<Dictionary<int, UsersList>> GetByIdsDictionaryAsync(List<int> userIds);
        Task<List<UsersList>> GetByFriendIdsAsync(List<int> friendIds);
        Task<(List<UsersList> Users, int TotalCount)> SearchExceptAsync(int currentUserId, int skip, int take);
        IQueryable<UsersList> Query();
    }
}