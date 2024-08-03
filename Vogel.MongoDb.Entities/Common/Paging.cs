namespace Vogel.MongoDb.Entities.Common
{
    public class Paging<T>
    {
        public List<T> Data { get; set; }
        public PagingInfo Info { get; set; }
    }
}
