using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Vogel.BuildingBlocks.EntityFramework;
using Vogel.MediaEngine.Infrastructure.EntityFramework.Constants;

namespace Vogel.MediaEngine.Infrastructure.EntityFramework
{
    public class MediaEngineDbContext : VogelDbContext<MediaEngineDbContext>
    {
        public MediaEngineDbContext(DbContextOptions<MediaEngineDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(MediaEngineDbConstants.Schema);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
