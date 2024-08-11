using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Domain.Medias;

namespace Vogel.Infrastructure.EntityFramework.Mapping
{
    public class MediaEntityTypeConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.ToTable(MediaTableConsts.TableName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(MediaTableConsts.IdLength);

            builder.Property(x => x.MimeType).HasMaxLength(MediaTableConsts.MimeTypeLength);

            builder.Property(x => x.UserId).HasMaxLength(MediaTableConsts.UserIdLength);

            builder.Property(x => x.File).HasMaxLength(MediaTableConsts.FileLength);

            builder.HasIndex(x => x.UserId);

            builder.AutoMapAuditing();
        }
    }
}
