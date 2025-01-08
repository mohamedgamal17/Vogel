using Bogus;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.Domain.Users;

namespace Vogel.Social.Application.Tests.Fakers
{
    public class MediaFaker : Faker<Picture>
    {
        public MediaFaker(User user)
        {
            RuleFor(x => x.File, Guid.NewGuid().ToString());
            RuleFor(x => x.UserId, user.Id);
        }
    }

}
