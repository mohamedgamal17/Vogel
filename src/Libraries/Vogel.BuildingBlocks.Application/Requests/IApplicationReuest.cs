using MediatR;
using Vogel.BuildingBlocks.Domain.Results;

namespace Vogel.BuildingBlocks.Application.Requests
{
    public interface IApplicationReuest<T> :  IRequest<Result<T>>
    {
    }
}
