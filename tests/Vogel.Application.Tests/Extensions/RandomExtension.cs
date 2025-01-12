using Bogus;
using MassTransit.SqlTransport.Topology;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Vogel.Application.Tests.Extensions
{
    public static class RandomExtension
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

        public static List<TEntity> PickRandom<TEntity>(this IEnumerable<TEntity> data, int count)
        {
            var faker = new Faker();

            int totalCount =  data.Count();

            int skipper = faker.Random.Int(0, totalCount);

            return  data
                .OrderBy(x => EF.Functions.Random())
                .Skip(skipper)
                .Take(count)
                .ToList();
        }

        public static TEntity? PickRandom<TEntity>(this IEnumerable<TEntity> data)
        {
            var faker = new Faker();

            int totalCount = data.Count();

            int skipper = faker.Random.Int(0, totalCount);


            return data
                .OrderBy(x => EF.Functions.Random())
                .FirstOrDefault();
        }
    }
}
