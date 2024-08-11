namespace Vogel.BuildingBlocks.Application.Requests
{
    public interface IUpdateCommand<TId, TResult> : ICommand<TResult>
    { 
         TId Id { get; }
         void SetId(TId id);
    }


}
