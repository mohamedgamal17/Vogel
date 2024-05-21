using Vogel.BuildingBlocks.Domain.Interfaces;

namespace Vogel.BuildingBlocks.Domain
{
    public abstract class Entity<TKey> : IEntity<TKey>
    {
        public TKey Id { get ; set ; }
    }
}
