using MediatR;
using Vogel.BuildingBlocks.Shared.Results;
namespace Vogel.BuildingBlocks.Application.Requests
{
    public interface IApplicationRequestHandler<TRequest, TResult> : MediatR.IRequestHandler<TRequest, Result<TResult>>
        where TRequest : IApplicationReuest<TResult>
    {
    }

    public interface IApplicationPiplineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
    {
    }

}
