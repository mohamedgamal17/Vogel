using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Messanger.Domain.Messages;

namespace Vogel.Messanger.Infrastructure.EntityFramework.Configuration
{
    public class MessageActivityEntityTypeConfiguration : IEntityTypeConfiguration<MessageActivity>
    {
        public void Configure(EntityTypeBuilder<MessageActivity> builder)
        {
            builder.ToTable(MessageActivityTableConsts.TableName);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasMaxLength(MessageActivityTableConsts.IdLength);
            builder.Property(x => x.SeenById).HasMaxLength(MessageActivityTableConsts.SeenByIdLength);
            builder.Property(x => x.MessageId).HasMaxLength(MessageActivityTableConsts.MessageIdLength);
            builder.Property(x => x.SeenAt).IsRequired();
            builder.AutoMapAuditing();
            builder.HasOne<Message>().WithMany().HasForeignKey(x => x.MessageId);
            builder.HasIndex(x => x.SeenById);
        }
    }
}
