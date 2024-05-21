using Vogel.BuildingBlocks.Domain.Events;

namespace Vogel.BuildingBlocks.Domain
{
    public class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
    {
        private readonly List<IEvent> _events = new List<IEvent>();
        public IReadOnlyList<IEvent> Events => _events.AsReadOnly();
        public void AppendEvent(IEvent @event) => _events.Add(@event);
    }
}
