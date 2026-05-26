using Chat_App.Models;
namespace Chat_App.Repositories
{
    public interface IMomentRepository
    {
        Task<List<Moment>> GetActiveMomentsAsync(List<int> userIds);
        Task<List<Moment>> GetMyMomentsAsync(int userId);
        Task<MomentView?> GetExistingViewAsync(int momentId, int viewerId);
        Task AddMomentAsync(Moment moment);
        Task AddViewAsync(MomentView view);
        Task<int> GetViewCountAsync(int momentId);
        Task<List<object>> GetMomentViewsAsync(int momentId);
        Task<List<Moment>> GetExpiredMomentsAsync();
        void RemoveMoment(Moment moment);
        Task SaveChangesAsync();
    }
}