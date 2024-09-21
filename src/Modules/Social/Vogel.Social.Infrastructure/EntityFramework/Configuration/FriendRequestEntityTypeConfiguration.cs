using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.Domain.Users;
namespace Vogel.Social.Infrastructure.EntityFramework.Configuration
{
    public class FriendRequestEntityTypeConfiguration : IEntityTypeConfiguration<FriendRequest>
    {
        public void Configure(EntityTypeBuilder<FriendRequest> builder)
        {
            builder.ToTable(FriendRequestTableConsts.TableName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(FriendRequestTableConsts.IdLength);

            builder.Property(x => x.SenderId).HasMaxLength(FriendRequestTableConsts.SenderIdLenght);

            builder.Property(x => x.ReciverId).HasMaxLength(FriendRequestTableConsts.ReciverIdLenght);

            builder.HasOne<User>().WithMany().HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>().WithMany().HasForeignKey(x => x.ReciverId).OnDelete(DeleteBehavior.Restrict);

            builder.AutoMapAuditing();
        }
    }
}
