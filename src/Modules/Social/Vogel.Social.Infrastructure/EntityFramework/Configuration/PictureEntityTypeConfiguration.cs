using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Social.Domain.Pictures;

namespace Vogel.Social.Infrastructure.EntityFramework.Configuration
{
    public class PictureEntityTypeConfiguration : IEntityTypeConfiguration<Picture>
    {
        public void Configure(EntityTypeBuilder<Picture> builder)
        {
            builder.ToTable(PictureTableConsts.TableName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(PictureTableConsts.IdLength);

            builder.Property(x => x.UserId).HasMaxLength(PictureTableConsts.UserIdLength);

            builder.Property(x => x.File).HasMaxLength(PictureTableConsts.FileLength);

            builder.HasIndex(x => x.UserId).IsUnique(false);

            builder.AutoMapAuditing();
        }
    }
}
