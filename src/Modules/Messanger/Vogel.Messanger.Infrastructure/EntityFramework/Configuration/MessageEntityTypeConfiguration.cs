using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain.Messages;
namespace Vogel.Messanger.Infrastructure.EntityFramework.Configuration
{
    public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable(MessageTableConst.TableName);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasMaxLength(MessageTableConst.IdLength);
            builder.Property(x => x.Content).HasMaxLength(MessageTableConst.ContentLength);
            builder.Property(x => x.ConversationId).HasMaxLength(MessageTableConst.ConversationIdLength);
            builder.Property(x => x.SenderId).HasMaxLength(MessageTableConst.SenderId);
            builder.HasIndex(x => x.SenderId);
            builder.HasOne<Conversation>().WithMany().HasForeignKey(x => x.ConversationId);
            builder.AutoMapAuditing();
        }
    }
}
