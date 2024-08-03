using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Common;
namespace Vogel.MongoDb.Entities.Extensions
{
    public static class IAggregateFluentExtensions
    {
        public static async Task<Paging<T>> ToPaged<T> (this IAggregateFluent<T> query , string? cursor = null ,int limit = 10 , bool ascending = false)
            where T : IMongoEntity
        {
            var orderedQuery = ascending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);

            if(cursor != null)
            {
                var filter = ascending ? Builders<T>.Filter.Gte(x => x.Id, cursor) : Builders<T>.Filter.Lte(x => x.Id, cursor);

                query = orderedQuery.Match(filter);
            }
            var data = await orderedQuery.Limit(limit).ToListAsync();

            var pagingInfo = await PreparePagingInfo(orderedQuery, cursor, limit, ascending);

            return new Paging<T>
            {
                Data = data,
                Info = pagingInfo
            };
        }

        private static async Task<PagingInfo> PreparePagingInfo<T>(IAggregateFluent<T> query, string? cursor = null, int limit = 10, bool ascending = false)
            where T : IMongoEntity
        {
            if (cursor!= null)
            {

                var previosFilter = ascending ? Builders<T>.Filter.Lt(x => x.Id, cursor)
                           : Builders<T>.Filter.Gt(x => x.Id, cursor);

                var nextFilter = ascending ? Builders<T>.Filter.Gt(x => x.Id, cursor)
                    : Builders<T>.Filter.Lt(x => x.Id, cursor);


                var next = await query.Match(nextFilter).Skip(limit - 1).FirstOrDefaultAsync();

                var previos = await query.Match(previosFilter).FirstOrDefaultAsync();

                return new PagingInfo(next?.Id, previos?.Id, ascending);
            }
            else
            {
                var next = await query.Skip(limit - 1).FirstOrDefaultAsync();

                return new PagingInfo(next?.Id, null, ascending);
            }
        }
    }
}
