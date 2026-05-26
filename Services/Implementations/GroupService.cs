using Chat_App.Models;
using Chat_App.Repositories;
using Chat_App.Services.Dtos;
using Chat_App.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Chat_App.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly IUserRepository _userRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IHiddenRepository _hiddenRepo;
        private readonly IFriendRequestRepository _friendRequestRepo;
        public GroupService(IUserRepository userRepo, IGroupRepository groupRepo, IHiddenRepository hiddenRepo, IFriendRequestRepository friendRequestRepo)
        {
            _userRepo = userRepo;
            _groupRepo = groupRepo;
            _hiddenRepo = hiddenRepo;
            _friendRequestRepo = friendRequestRepo;
        }
        public async Task<GroupCreateResult> CreateGroup(GroupCredentials group, int currentUserId)
        {
            group.UserIds.Add(currentUserId);
            group.AdminIds?.Add(currentUserId);
            await _groupRepo.AddAsync(group);
            return new GroupCreateResult { Success = true };
        }
        public async Task ToggleHideGroup(int currentUserId, int groupId)
        {
            var existing = await _hiddenRepo.GetHiddenGroupAsync(currentUserId, groupId);
            if (existing != null)
                _hiddenRepo.RemoveHiddenGroup(existing);
            else
                await _hiddenRepo.AddHiddenGroupAsync(new HiddenGroup { UserId = currentUserId, GroupId = groupId });
            await _hiddenRepo.SaveChangesAsync();
        }
        public async Task ToggleHideUser(int currentUserId, int hiddenUserId)
        {
            var existing = await _hiddenRepo.GetHiddenChatAsync(currentUserId, hiddenUserId);
            if (existing != null)
                _hiddenRepo.RemoveHiddenChat(existing);
            else
                await _hiddenRepo.AddHiddenChatAsync(new HiddenChat { UserId = currentUserId, HiddenUserId = hiddenUserId });
            await _hiddenRepo.SaveChangesAsync();
        }
        public (GroupCredentials? Group, List<UsersList> Members, UsersList? CurrentUser) GetGroupChatData(int groupId, int currentUserId)
        {
            var group = _groupRepo.Query().FirstOrDefault(g => g.GroupId == groupId);
            if (group == null) return (null, new List<UsersList>(), null);
            var members = _userRepo.Query().Where(u => group.UserIds.Contains(u.Id)).ToList();
            var currentUser = _userRepo.Query().FirstOrDefault(u => u.Id == currentUserId);
            return (group, members, currentUser);
        }
        public (bool IsFriend, string? RequestStatus, bool IsSender, UsersList? Receiver, UsersList? CurrentUser, bool IsBlocked, string? Relationship) GetChatData(int id, int currentUserId)
        {
            var friendship = _friendRequestRepo.Query()
                .FirstOrDefault(f =>
                    (f.SenderId == currentUserId && f.ReceiverId == id) ||
                    (f.SenderId == id && f.ReceiverId == currentUserId));
            bool isFriend = friendship != null && friendship.Status == "Accepted";
            string? requestStatus = null;
            bool isSender = false;
            if (!isFriend && friendship != null)
            {
                requestStatus = friendship.Status;
                isSender = friendship.SenderId == currentUserId;
            }
            var receiver = _userRepo.Query().FirstOrDefault(u => u.Id == id);
            var currentUser = _userRepo.Query().FirstOrDefault(u => u.Id == currentUserId);
            bool isBlocked = currentUser?.BlockedUsers != null &&
                             ("," + currentUser.BlockedUsers + ",").Contains("," + id + ",");
            return (isFriend, requestStatus, isSender, receiver, currentUser, isBlocked, friendship?.Bond ?? "Unknown");
        }
    }
}