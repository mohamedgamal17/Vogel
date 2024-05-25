using Microsoft.EntityFrameworkCore;
using Vogel.BuildingBlocks.EntityFramework;

namespace Vogel.Infrastructure.EntityFramework
{
    public class ApplicationDbContext : VogelDbContext<ApplicationDbContext>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {

        }
    }
}
