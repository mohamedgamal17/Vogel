using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Content.Domain.Posts;
namespace Vogel.Content.Infrastructure.EntityFramework.Configuration
{
    public class PostReactionEntityTypeConfiguration : IEntityTypeConfiguration<PostReaction>
    {
        public void Configure(EntityTypeBuilder<PostReaction> builder)
        {
            builder.ToTable(PostReactionTableConsts.TableName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(PostReactionTableConsts.IdLength);

            builder.Property(x => x.UserId).HasMaxLength(PostReactionTableConsts.UserIdLength);

            builder.Property(x => x.PostId).HasMaxLength(PostReactionTableConsts.PostIdLength);

            builder.HasOne<Post>().WithMany().HasForeignKey(x => x.PostId);

            builder.HasIndex(x => x.UserId);

            builder.AutoMapAuditing();
        }
    }
}
