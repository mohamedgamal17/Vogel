using Vogel.BuildingBlocks.Domain.Repositories;

namespace Vogel.Social.Domain
{
    public interface ISocialRepository<T> : IRepository<T> where T : class
    {
    }
}
