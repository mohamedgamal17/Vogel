using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.Domain.Auditing;
using Vogel.BuildingBlocks.EntityFramework.Converters;

namespace Vogel.BuildingBlocks.EntityFramework.Extensions
{
    public static class EntityBuilderExtensions
    {
        public static void AutoMapAuditing<TEntity>(this EntityTypeBuilder<TEntity> builder)where TEntity : class ,IAuditedEntity        
        {
            builder.Property(x => x.CreationTime).HasConversion<DateTimeValueConverter>();

            builder.Property(x => x.CreatorId).HasMaxLength(256).IsRequired(false);

            builder.Property(x => x.ModificationTime).HasConversion<DateTimeValueConverter>();

            builder.Property(x => x.ModifierId).HasMaxLength(256).IsRequired(false);

            builder.Property(x => x.DeletionTime).HasConversion<NullableDateTimeValueConverter>().IsRequired(false);

            builder.Property(x => x.DeletorId).HasMaxLength(256).IsRequired(false);

            builder.HasIndex(x => x.CreatorId);

            builder.HasIndex(x => x.ModifierId);

            builder.HasIndex(x => x.DeletorId);
        }
    }
}
