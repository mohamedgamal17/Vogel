using MediatR;
using Vogel.BuildingBlocks.Shared.Results;
namespace Vogel.BuildingBlocks.Application.Behaviours
{
    public interface IApplicationPiplineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
      where TRequest : notnull
    {
    }
}
