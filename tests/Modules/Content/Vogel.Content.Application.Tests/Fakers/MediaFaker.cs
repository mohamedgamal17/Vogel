using Bogus;
using Vogel.Content.Domain.Medias;

namespace Vogel.Content.Application.Tests.Fakers
{
    public class MediaFaker :Faker<Media>
    {
        public MediaFaker(string userId)
        {
            RuleFor(x => x.File, _ => Guid.NewGuid().ToString());
            RuleFor(x => x.UserId, userId);
            RuleFor(x => x.MimeType, "image/apng");
        }
    }
}
