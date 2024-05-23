namespace Vogel.BuildingBlocks.MongoDb
{
    public interface IMongoEntity
    {

    }

    public interface IMongoEntity<TKey> : IMongoEntity
    {
        TKey Id { get; set; }
    }
    public abstract class MongoEntity<TKey> : IMongoEntity<TKey>
    {
        public TKey Id { get; set; }
    }



}
