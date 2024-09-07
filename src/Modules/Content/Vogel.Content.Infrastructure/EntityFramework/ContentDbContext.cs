using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Vogel.BuildingBlocks.EntityFramework;
using Vogel.Content.Infrastructure.EntityFramework.Constants;

namespace Vogel.Content.Infrastructure.EntityFramework
{
    public class ContentDbContext : VogelDbContext<ContentDbContext>
    {
        public ContentDbContext(DbContextOptions<ContentDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(ContentDbConstants.Schema);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
