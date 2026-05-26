using Chat_App.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat_App
{
    public class ApplicationDBContext : DbContext
    {

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        public DbSet<UsersList> user { get; set; }
        public DbSet<Message> message { get; set; }
        public DbSet<GroupCredentials> group { get; set; }
        public DbSet<GroupMessage> groupMessage { get; set; }
        public DbSet<HiddenChat> hiddenChat { get; set; }
        public DbSet<HiddenGroup> hiddenGroup { get; set; }
        public DbSet<PushSubscription> pushSubscription { get; set; }
        public DbSet<GroupMessageRecipient> groupMessageRecipient { get; set; }
        public DbSet<FriendRequest> friendRequests { get; set; }
        public DbSet<ChatBackground> chatBackground { get; set; }
        public DbSet<Moment> moments { get; set; }
        public DbSet<MomentView> momentView { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasIndex(m => new { m.SenderId, m.ReceiverId, m.SentAt })
                    .HasDatabaseName("IX_message_SenderId_ReceiverId_SentAt")
                    .IsDescending(false, false, true);
            });

            modelBuilder.Entity<GroupMessage>(entity =>
            {
                entity.HasIndex(m => new { m.GroupId, m.SentAt })
                    .HasDatabaseName("IX_groupMessage_GroupId_SentAt")
                    .IsDescending(false, true);
            });
        }
    }
}
