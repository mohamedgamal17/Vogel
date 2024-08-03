using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.BuildingBlocks.EntityFramework.Extensions;
using Vogel.Domain.Friendship;
using Vogel.Domain.Users;
namespace Vogel.Infrastructure.EntityFramework.Mapping
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(UserTableConsts.IdLength);

            builder.Property(x => x.FirstName).HasMaxLength(UserTableConsts.FirstNameLength);

            builder.Property(x => x.LastName).HasMaxLength(UserTableConsts.LastNameLength);

            builder.Property(x => x.BirthDate).HasColumnType("date");

            builder.Property(x => x.AvatarId).HasMaxLength(256);

            builder.HasOne(x=> x.Avatar).WithOne().HasForeignKey<User>(x => x.AvatarId);

            builder.AutoMapAuditing();
        }
    }
}
