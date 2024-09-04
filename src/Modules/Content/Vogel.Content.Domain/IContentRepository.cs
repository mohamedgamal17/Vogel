using Vogel.BuildingBlocks.Domain.Repositories;

namespace Vogel.Content.Domain
{
    public interface IContentRepository<T> : IRepository<T> where T : class
    {
    }
}
