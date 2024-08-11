namespace Vogel.BuildingBlocks.Application.Requests
{
    public class UpdateCommand<TId, TResult> : IUpdateCommand<TId, TResult>
    {
        private TId _id = default(TId);
        public TId Id => _id;

        public void SetId(TId id)
        {
            _id = id;
        }
    }
}
