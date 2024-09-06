using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Content.Domain.Posts;
namespace Vogel.Content.Infrastructure.EntityFramework.Configuration
{
    public class PostEntityTypeConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable(PostTableConsts.TableName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(PostTableConsts.IdLength);

            builder.Property(x => x.Caption).HasMaxLength(PostTableConsts.CaptionLength);

            builder.Property(x => x.MediaId).HasMaxLength(PostTableConsts.CaptionLength);

            builder.Property(x => x.UserId).HasMaxLength(PostTableConsts.UserIdLength);

            builder.AutoMapAuditing();
        }
    }
}
