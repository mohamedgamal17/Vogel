using MediatR;
using Vogel.Domain.Utils;

namespace Vogel.Application.Common.Interfaces
{
    public interface IApplicationReuest<T> : IRequest<Result<T>>
    {
    }
    public interface ICommand<T> : IApplicationReuest<T> { }
    public interface ICommand : ICommand<Unit> { }
    public interface IQuery<T> : IApplicationReuest<T> { }
}
