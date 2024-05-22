namespace Vogel.BuildingBlocks.Application.Uow
{
    public interface IUnitOfWorkManager
    {
        IUnitOfWork? Current { get; }
        Task<IUnitOfWork> BeginAsync(CancellationToken cancellationToken  = default);
    }
}
