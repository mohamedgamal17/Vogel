using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.Domain.Users;
namespace Vogel.Social.Infrastructure.EntityFramework.Configuration
{
    public class FriendEntityTypeConfiguration : IEntityTypeConfiguration<Friend>
    {
        public void Configure(EntityTypeBuilder<Friend> builder)
        {
            builder.ToTable(FriendTableConsts.TableName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(FriendTableConsts.IdLength);

            builder.Property(x => x.SourceId).HasMaxLength(FriendTableConsts.SourceIdLenght);

            builder.Property(x => x.TargetId).HasMaxLength(FriendTableConsts.TargetIdLenght);

            builder.HasOne<User>().WithMany().HasForeignKey(x => x.SourceId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>().WithMany().HasForeignKey(x => x.TargetId).OnDelete(DeleteBehavior.Restrict);

            builder.AutoMapAuditing();
        }
    }
}
