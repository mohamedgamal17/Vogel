using MediatR;

namespace Vogel.BuildingBlocks.Application.Requests
{
    public interface ICommand<T> : IApplicationReuest<T>
    {
    }

    public interface ICommand : ICommand<Unit> { }
}
