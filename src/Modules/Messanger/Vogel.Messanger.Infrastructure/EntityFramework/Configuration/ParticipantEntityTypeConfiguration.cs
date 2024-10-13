using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Messanger.Domain.Conversations;
namespace Vogel.Messanger.Infrastructure.EntityFramework.Configuration
{
    public class ParticipantEntityTypeConfiguration : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.ToTable(ParticipantTableConst.TableName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(ParticipantTableConst.IdLength);

            builder.Property(x => x.UserId).HasMaxLength(ParticipantTableConst.UserIdLength).IsRequired(false);

            builder.Property(x => x.ConversationId).HasMaxLength(ParticipantTableConst.ConversationIdLength).IsRequired(false);

            builder.AutoMapAuditing();
        }
    }
}
