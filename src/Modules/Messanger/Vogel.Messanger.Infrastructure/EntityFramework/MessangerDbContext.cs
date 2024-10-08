using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Vogel.BuildingBlocks.EntityFramework;
using Vogel.Messanger.Infrastructure.EntityFramework.Constants;
namespace Vogel.Messanger.Infrastructure.EntityFramework
{
    public class MessangerDbContext : VogelDbContext<MessangerDbContext>
    {
        public MessangerDbContext(DbContextOptions<MessangerDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(MessangerDbConstants.Schema);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
