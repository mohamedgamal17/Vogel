using Vogel.Application.Medias.Dtos;
using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.Application.Users.Dtos
{
    public class UserDto : EntityDto<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string BirthDate { get; set; }
        public string? AvatarId { get; set; }
        public MediaAggregateDto? Avatar { get; set; }
    }
}
