using Vogel.BuildingBlocks.Domain.Auditing;

namespace Vogel.Social.Domain.Pictures
{
    public class Picture : AuditedAggregateRoot<string>
    {
        public Picture()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string File { get; set; }
        public string UserId { get; set; }
    }
}
