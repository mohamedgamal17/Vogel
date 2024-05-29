﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Domain.Posts;

namespace Vogel.Infrastructure.EntityFramework.Mapping
{
    public class CommentEntityTypeConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable(CommentTableConsts.TableName);

            builder.Property(x=> x.Id).HasMaxLength(CommentTableConsts.IdLength);

            builder.Property(x=> x.Content).HasMaxLength(CommentTableConsts.ContentLength);

            builder.Property(x=> x.PostId).HasMaxLength(CommentTableConsts.PostIdLength);

            builder.Property(x=> x.UserId).HasMaxLength(CommentTableConsts.UserIdLength);

            builder.HasOne<Post>().WithMany().HasForeignKey(x => x.PostId);

            builder.HasIndex(x => x.UserId);

            builder.AutoMapAuditing();

        }
    }
}