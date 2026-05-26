using Chat_App.Models;
namespace Chat_App.Repositories
{
    public interface IGroupRepository
    {
        Task<GroupCredentials?> GetByIdAsync(int groupId);
        Task<List<GroupCredentials>> GetUserGroupsAsync(int userId);
        Task AddAsync(GroupCredentials group);
        Task SaveChangesAsync();
        IQueryable<GroupCredentials> Query();
    }
}