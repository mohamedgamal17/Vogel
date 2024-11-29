using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Messanger.Domain.Messages;

namespace Vogel.Messanger.Infrastructure.EntityFramework.Configuration
{
    public class MessageLogEntityTypeConfiguration : IEntityTypeConfiguration<MessageLog>
    {
        public void Configure(EntityTypeBuilder<MessageLog> builder)
        {
            builder.ToTable(MessageLogTableConsts.TableName);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasMaxLength(MessageLogTableConsts.IdLength);
            builder.Property(x => x.SeenById).HasMaxLength(MessageLogTableConsts.SeenByIdLength);
            builder.Property(x => x.MessageId).HasMaxLength(MessageLogTableConsts.MessageIdLength);
            builder.Property(x => x.SeenAt).IsRequired();
            builder.AutoMapAuditing();
            builder.HasOne<Message>().WithMany().HasForeignKey(x => x.MessageId);
            builder.HasIndex(x => x.SeenById);
        }
    }
}
