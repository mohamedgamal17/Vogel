using Vogel.BuildingBlocks.Domain.Events;

namespace Vogel.BuildingBlocks.Domain
{
    public interface IAggregateRoot : IEntity
    {
        IReadOnlyList<IEvent> Events { get; }
        void ClearDomainEvents();
    }
    public interface IAggregateRoot<TKey> : IAggregateRoot, IEntity<TKey>
    {

    }
}
