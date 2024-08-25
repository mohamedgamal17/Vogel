using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.Domain.Users;
namespace Vogel.Social.Infrastructure.EntityFramework.Configuration
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(UserTableConsts.TableName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(UserTableConsts.IdLength);

            builder.Property(x => x.FirstName).HasMaxLength(UserTableConsts.FirstNameLength);

            builder.Property(x => x.LastName).HasMaxLength(UserTableConsts.LastNameLength);

            builder.Property(x => x.BirthDate).HasColumnType("date");

            builder.Property(x => x.AvatarId).HasMaxLength(256);

            builder.HasOne<Picture>().WithOne().HasForeignKey<User>(x => x.AvatarId);

            builder.AutoMapAuditing();
        }
    }
}
