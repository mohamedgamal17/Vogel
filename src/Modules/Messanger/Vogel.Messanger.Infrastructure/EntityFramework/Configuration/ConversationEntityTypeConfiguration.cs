using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain.Messages;
namespace Vogel.Messanger.Infrastructure.EntityFramework.Configuration
{
    public class ConversationEntityTypeConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.ToTable(ConversationTableConst.TableName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(ConversationTableConst.IdLength);

            builder.Property(x => x.Name).HasMaxLength(ConversationTableConst.Name).IsRequired(false);

            builder.HasMany<Message>().WithOne().HasForeignKey(x => x.ConversationId);

            builder.AutoMapAuditing();
        }
    }
}
