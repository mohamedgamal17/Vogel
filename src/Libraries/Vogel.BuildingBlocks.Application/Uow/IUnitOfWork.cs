using Vogel.BuildingBlocks.Domain.Events;

namespace Vogel.BuildingBlocks.Application.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        void AddDomainEvent(IEvent @event);

        void AddRangeDomainEvents(List<IEvent> @events);

        Task CommitAsync(CancellationToken cancellationToken = default);

        Task RollbackAsync(CancellationToken cancellationToken);

    }
}
