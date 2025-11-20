namespace Vogel.BuildingBlocks.MongoDb
{
    public interface IMongoOwnedEntity<T>
    {
        T UserId { get; set; }
        bool IsOwnedBy(T userId);
    }

    public interface IMongoOwnedEntity : IMongoOwnedEntity<string>
    {

    }
}
