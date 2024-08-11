using Vogel.BuildingBlocks.Domain;
using Vogel.BuildingBlocks.Domain.Auditing;

namespace Vogel.Domain.Posts
{
    public class Post : AuditedAggregateRoot<string>
    {
        public Post()
        {
            Id = Guid.NewGuid().ToString();
        }


        public string Caption { get; set; }
        public string? MediaId { get; set; }
        public string UserId { get; set; }

    }



}
