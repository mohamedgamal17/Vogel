using CaseConverter;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;
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

        public static IAggregateFluent<TResult> SubPaged<TView,TResult> (this IAggregateFluent<TView> query , Expression<Func<TResult, object>> @as ,string propertyPerfix = "_" , int limit = 10 , bool asc  = false)
        {
            var targetProperty = string.Format("{0}{1}", propertyPerfix, ExtractPropertyName(@as));

            var @asProperty = ExtractPropertyName(@as);


            var sortedQuery = query.AppendStage<BsonDocument>(new BsonDocument("$set", new BsonDocument()
            {
                { 
                    $"{targetProperty}" , new BsonDocument("$sortArray" ,new BsonDocument() 
                    {
                        {"input", $"${targetProperty}" },
                        {"sortBy",new BsonDocument()
                            {
                               { "id" , asc ? 1 : -1 }
                            }
                        }
                    })
                
                }
               
            }));

            var newQuery = sortedQuery.AppendStage<TResult>(new BsonDocument("$addFields", new BsonDocument()
            {
                {
                    $"{asProperty}", new BsonDocument()
                    {
                        { "data",  new  BsonDocument("$slice", new BsonArray() { $"${targetProperty}" , limit })  },
                        {
                            "info" , new BsonDocument()
                            {
                                {
                                    "nextCursor" , new BsonDocument("$arrayElemAt" , new BsonArray () {$"${targetProperty}", limit})

                                }
                            }
                        }
                    }
                }

            }));


            newQuery = newQuery.AppendStage<TResult>(new BsonDocument("$unset", $"{targetProperty}"));

            return newQuery;
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

        private static string ExtractPropertyName<T>(Expression<Func<T, object>> expression)
        {
            if(expression.NodeType != ExpressionType.Lambda)
            {
                throw new InvalidOperationException("Property name extraction expression should be of type lambda expression");
            }
           

            if(expression.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new InvalidOperationException("Lambda inner expression should be of type of member access expression");
            }

            var memberExpression = (MemberExpression)expression.Body;

            return memberExpression.Member.Name.ToCamelCase();
        }

    }
}
