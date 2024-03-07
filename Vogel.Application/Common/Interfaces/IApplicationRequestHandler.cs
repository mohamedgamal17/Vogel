using MediatR;
using MediatR.Pipeline;
using Vogel.Domain.Utils;

namespace Vogel.Application.Common.Interfaces
{
    public interface IApplicationRequestHandler<TRequest,TResult> : MediatR.IRequestHandler<TRequest,Result<TResult>>
        where TRequest : IApplicationReuest<TResult>
    { 

    }

    public interface IApplicationPiplineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
        //where TRequest : IApplicationReuest<TResponse>
    {
    }


}
