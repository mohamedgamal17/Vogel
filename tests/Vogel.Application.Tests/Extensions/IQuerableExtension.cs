using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Vogel.Application.Tests.Extensions
{
    public static class IQuerableExtension
    {
        public static async Task<List<TEntity>> PickRandom<TEntity>(this IQueryable<TEntity> query,int count)
            where TEntity : class
        {
            var faker = new Faker();

            int totalCount = await query.CountAsync();

            int skipper = faker.Random.Int(0, totalCount);

            return await query
                .OrderBy(x => EF.Functions.Random())
                .Skip(skipper)
                .Take(count)
                .ToListAsync();
        }

        public static async Task<TEntity?> PickRandom<TEntity>(this IQueryable<TEntity> query )
        {
            return await query
                .OrderBy(x => EF.Functions.Random())
                .FirstOrDefaultAsync();
        }
    }
}
