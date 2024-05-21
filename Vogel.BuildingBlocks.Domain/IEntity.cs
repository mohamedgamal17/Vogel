namespace Vogel.BuildingBlocks.Domain
{
    public interface IEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}
