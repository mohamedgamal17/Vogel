using Microsoft.EntityFrameworkCore;

namespace Vogel.MediaEngine.Infrastructure.EntityFramework
{
    public class MediaEngineDbContext : DbContext
    {
        public MediaEngineDbContext(DbContextOptions<MediaEngineDbContext> options)
            : base(options)
        {
        }
    }
}
