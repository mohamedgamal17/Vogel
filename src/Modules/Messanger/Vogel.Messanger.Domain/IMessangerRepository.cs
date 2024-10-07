using Vogel.BuildingBlocks.Domain.Repositories;

namespace Vogel.Messanger.Domain
{
    internal interface IMessangerRepository<T> : IRepository<T> where T: class 
    {
    }
}
