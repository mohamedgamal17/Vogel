using Vogel.BuildingBlocks.Domain.Auditing;

namespace Vogel.Social.Domain.Pictures
{
    public class Picture : OwnedAuditedAggregateRoot<string>
    {
        public Picture()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string File { get; set; }
    }
}
