using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Vogel.BuildingBlocks.Domain.Events;
namespace Vogel.BuildingBlocks.Application.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContextTransaction _dbContextTransaction;

        private readonly IServiceProvider _serviceProvider;

        private  bool _isCompleted = false;

        private  bool _isDisposed = false;

        private bool _isRolledBack = false;


        private List<IEvent> _domainEvents = new List<IEvent>();

        public UnitOfWork(IDbContextTransaction dbContextTransaction, IServiceProvider serviceProvider )
        {
            _dbContextTransaction = dbContextTransaction;
            _serviceProvider = serviceProvider;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if(_isCompleted || _isRolledBack)
            {
                return;
            }

            var publisher = _serviceProvider.GetRequiredService<IMediator>();

            foreach (var notification in _domainEvents)
            {
                await publisher.Publish(notification);
            }

            _domainEvents.Clear();

            await _dbContextTransaction.CommitAsync(cancellationToken);

            _isCompleted = true;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _dbContextTransaction.Dispose();


            _isDisposed = true;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_isCompleted || _isRolledBack)
            {
                return;
            }




            await _dbContextTransaction.RollbackAsync(cancellationToken);

            _isRolledBack = true;
        }

        public void AddDomainEvent(IEvent @event)
        {
            if(_isCompleted || _isRolledBack)
            {
                return;
            }

            _domainEvents.Add(@event);
        }

        public void AddRangeDomainEvents(List<IEvent> events)
        {
            if (_isCompleted || _isRolledBack)
            {
                return;
            }

            _domainEvents.AddRange(events);
        }
    }
}
