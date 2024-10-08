using Vogel.BuildingBlocks.Domain.Repositories;

namespace Vogel.Messanger.Domain
{
    public interface IMessangerRepository<T> : IRepository<T> where T: class 
    {
    }
}
