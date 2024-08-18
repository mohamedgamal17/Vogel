using MediatR;
using Vogel.BuildingBlocks.Shared.Results;

namespace Vogel.BuildingBlocks.Application.Requests
{
    public interface IApplicationReuest<T> :  IRequest<Result<T>>
    {
    }
}
