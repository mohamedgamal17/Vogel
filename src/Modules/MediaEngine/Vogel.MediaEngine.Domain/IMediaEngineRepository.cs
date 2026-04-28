using Vogel.BuildingBlocks.Domain.Repositories;

namespace Vogel.MediaEngine.Domain
{
    public interface IMediaEngineRepository<T> : IRepository<T> where T : class
    {
    }
}
