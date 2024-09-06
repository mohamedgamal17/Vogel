using Microsoft.EntityFrameworkCore;
using Vogel.BuildingBlocks.EntityFramework;

namespace Vogel.Content.Infrastructure.EntityFramework
{
    public class ContentDbContext : VogelDbContext<ContentDbContext>
    {
        public ContentDbContext(DbContextOptions<ContentDbContext> options) : base(options)
        {

        }
    }
}
