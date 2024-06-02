using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Vogel.BuildingBlocks.EntityFramework.Interceptors;

namespace Vogel.BuildingBlocks.EntityFramework
{
    public abstract class VogelDbContext<TContext> :  DbContext where TContext : DbContext
    {

        public VogelDbContext(DbContextOptions<TContext> options) : base(options)
        {
        }
    }
}
