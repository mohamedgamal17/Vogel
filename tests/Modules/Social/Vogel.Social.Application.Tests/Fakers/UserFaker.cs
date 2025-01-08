using Bogus;
using Vogel.Social.Domain.Users;
using Vogel.Social.Shared.Common;

namespace Vogel.Social.Application.Tests.Fakers
{
    public class UserFaker : Faker<User>
    {
        public UserFaker()
        {
            RuleFor(x => x.FirstName, f => f.Name.FirstName());
            RuleFor(x => x.LastName, f => f.Name.LastName());
            RuleFor(x => x.BirthDate, f => f.Date.Past(40, DateTime.Now.AddYears(-18)));
            RuleFor(x => x.Gender, f => f.PickRandom<Gender>());
        }
    }

}
