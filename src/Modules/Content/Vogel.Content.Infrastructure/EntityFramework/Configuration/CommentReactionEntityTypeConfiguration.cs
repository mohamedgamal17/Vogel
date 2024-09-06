using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Content.Domain.Comments;

namespace Vogel.Content.Infrastructure.EntityFramework.Configuration
{
    public class CommentReactionEntityTypeConfiguration : IEntityTypeConfiguration<CommentReaction>
    {
        public void Configure(EntityTypeBuilder<CommentReaction> builder)
        {
            builder.ToTable(CommentReactionTableConsts.TableName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(CommentReactionTableConsts.IdLength);

            builder.Property(x => x.CommentId).HasMaxLength(CommentReactionTableConsts.CommentIdLength);

            builder.Property(x => x.UserId).HasMaxLength(CommentReactionTableConsts.UserIdLength);

            builder.HasOne<Comment>().WithMany().HasForeignKey(x => x.CommentId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.UserId);

            builder.AutoMapAuditing();

        }
    }
}
