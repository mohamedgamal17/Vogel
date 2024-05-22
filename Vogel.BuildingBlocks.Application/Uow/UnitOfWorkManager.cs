using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Vogel.BuildingBlocks.Application.Uow
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly AmbientUnitOfWork _ambientUnitOfWork;
        public UnitOfWorkManager(IServiceProvider serviceProvider, AmbientUnitOfWork ambientUnitOfWork)
        {
            _serviceProvider = serviceProvider;
            _ambientUnitOfWork = ambientUnitOfWork;
        }

        public IUnitOfWork? Current => _ambientUnitOfWork.GetCurrentUnitOfWork();

        public async Task<IUnitOfWork> BeginAsync(CancellationToken cancellationToken = default)
        {
            var dbContext = _serviceProvider.GetRequiredService<DbContext>();

            var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var uow = new UnitOfWork(transaction, _serviceProvider);

            _ambientUnitOfWork.SetCurrentUnitOfWork(uow);

            return uow;
        }
    }

    public class AmbientUnitOfWork
    {
        private AsyncLocal<IUnitOfWork> _currentUnitOfWork = new AsyncLocal<IUnitOfWork>();

        public void SetCurrentUnitOfWork(IUnitOfWork unitOfWork)
        {
            _currentUnitOfWork.Value = unitOfWork;
        }


        public IUnitOfWork? GetCurrentUnitOfWork() => _currentUnitOfWork.Value;
    }

}
