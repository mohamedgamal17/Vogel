using Vogel.BuildingBlocks.Domain;
using Vogel.BuildingBlocks.Domain.Auditing;
namespace Vogel.Content.Domain.Posts
{
    public class Post : OwnedAuditedAggregateRoot<string>
    {
        public Post()
        {
            Id = Guid.NewGuid().ToString();
        }


        public string Caption { get; set; }
        public string? MediaId { get; set; }
    }



}
