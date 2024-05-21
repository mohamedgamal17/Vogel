using Vogel.BuildingBlocks.Domain.Events;

namespace Vogel.BuildingBlocks.Domain.Interfaces
{
    public interface IAggregateRoot<TKey> : IEntity<TKey>
    {
        IReadOnlyList<IEvent> Events { get;  }
        void AppendEvent(IEvent @event);
    }
}
