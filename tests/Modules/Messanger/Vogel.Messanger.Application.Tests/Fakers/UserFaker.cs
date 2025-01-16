using Bogus;
using Vogel.Social.Shared.Common;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Messanger.Application.Tests.Fakers
{
    public class UserFaker : Faker<UserDto>
    {
        public UserFaker()
        {
            RuleFor(x => x.Id, _ => Guid.NewGuid().ToString());
            RuleFor(x => x.FirstName, f => f.Name.FirstName());
            RuleFor(x => x.LastName, f => f.Name.LastName());
            RuleFor(x => x.Gender, f => f.PickRandom<Gender>());
            RuleFor(x => x.BirthDate, f => f.Date.Past(40, DateTime.Now.AddYears(-18)).ToString());
        }
    }
}
