using Chat_App.Services.Dtos;

namespace Chat_App.Services.Interfaces
{
    public interface IFriendService
    {
        Task<FriendRequestResult> SendFriendRequest(int currentUserId, int receiverId);
        Task AcceptFriendRequest(int currentUserId, int requestId);
        Task RejectFriendRequest(int currentUserId, int requestId);
        Task<List<PendingFriendRequestDto>> GetPendingRequests(int currentUserId);
        Task<List<SentFriendRequestDto>> GetSentRequests(int currentUserId);
        Task<FriendshipStatusDto> GetFriendshipStatus(int currentUserId, int userId);
        Task<UserSearchResultDto> GetAllUsersForFriendRequest(int currentUserId, int skip = 0, int take = 15);
        Task<List<FriendDto>> GetFriends(int currentUserId);
        Task UpdateRelationship(int currentUserId, int userId, string relationship);
        Task CancelFriendRequest(int currentUserId, int requestId);
    }
}
