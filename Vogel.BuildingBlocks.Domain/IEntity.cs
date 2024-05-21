namespace Vogel.BuildingBlocks.Domain.Interfaces
{
    public interface IEntity <TKey>
    {
        public TKey Id { get; set; }
    }
}
