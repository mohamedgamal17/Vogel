using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.Shared.Models;
namespace Vogel.BuildingBlocks.MongoDb.Extensions
{
    public static class IAggregateFluentExtensions
    {
        public static async Task<Paging<T>> ToPaged<T>(this IAggregateFluent<T> query, string? cursor = null, int limit = 10, bool ascending = false)
            where T : IMongoEntity
        {
            var orderedQuery = ascending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);

            if (cursor != null)
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

        public static Paging<T> ToPaged<T>(this IEnumerable<T> data , string? cursor = null , int limit =10, bool ascending = false)
             where T : IMongoEntity
        {
            var orderedData = ascending ? data.OrderBy(x => x.Id) : data.OrderByDescending(x => x.Id);

            var filterdData = orderedData.AsEnumerable();

            if(cursor != null)
            {

                filterdData = ascending ? filterdData.Where(x => x.Id.CompareTo(cursor) >= 0) : filterdData.Where(x => x.Id.CompareTo(cursor) <= 0);
            }

            var pagingInfo = PreparePagingInfo(filterdData, cursor, limit, ascending);

            return new Paging<T>
            {
                Data = filterdData.ToList(),
                Info = pagingInfo
            };
        }

        private static async Task<PagingInfo> PreparePagingInfo<T>(IAggregateFluent<T> query, string? cursor = null, int limit = 10, bool ascending = false)
            where T : IMongoEntity
        {
            if (cursor != null)
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

        private static PagingInfo PreparePagingInfo<T> (this IEnumerable<T> data, string? cursor = null
            , int limit= 10 , bool ascending = false)
            where T  : IMongoEntity
        {
            if(cursor != null)
            {
                var previous = ascending ? data.Where(x => x.Id.CompareTo(cursor) < 0).FirstOrDefault() : data.Where(x => x.Id.CompareTo(cursor) > 0).FirstOrDefault();
                var next = ascending ? data.Where(x => x.Id.CompareTo(cursor) > 0).FirstOrDefault() : data.Where(x => x.Id.CompareTo(cursor) < 0).FirstOrDefault();

                return new PagingInfo(next?.Id, previous?.Id, ascending);
            }
            else
            {
                var next = data.Skip(limit - 1).FirstOrDefault();

                return new PagingInfo(next?.Id, null, ascending);
            }
        }
    }
}
