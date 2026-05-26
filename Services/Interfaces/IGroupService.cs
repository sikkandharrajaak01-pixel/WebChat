using Chat_App.Models;
using Chat_App.Services.Dtos;

namespace Chat_App.Services.Interfaces
{
    public interface IGroupService
    {
        Task<GroupCreateResult> CreateGroup(GroupCredentials group, int currentUserId);
        Task ToggleHideGroup(int currentUserId, int groupId);
        Task ToggleHideUser(int currentUserId, int hiddenUserId);
        (GroupCredentials? Group, List<UsersList> Members, UsersList? CurrentUser) GetGroupChatData(int groupId, int currentUserId);
        (bool IsFriend, string? RequestStatus, bool IsSender, UsersList? Receiver, UsersList? CurrentUser, bool IsBlocked, string? Relationship) GetChatData(int id, int currentUserId);
    }
}
