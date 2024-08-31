using NUnit.Framework;
using System.Security.Claims;
using Vogel.BuildingBlocks.Infrastructure.Security;

namespace Vogel.Application.Tests.Services
{
    public class FakeUserService
    {
        private ApplicationUser? _currentUser = null;

        private ClaimsPrincipal? _currentUserPrincipal = null;
        public bool IsAuthenticated => _currentUser != null;

        private readonly object _obj = new object();
        public void Login()
        {
            Login(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), new List<string>());
        }


        public void Login(string id , string username , List<string> roles)
        {
            lock (_obj)
            {
                _currentUser = new ApplicationUser
                {
                    Id = id,
                    UserName = username,
                    Roles = roles
                };

                var princibal = new ClaimsPrincipal();

                var identity = new ClaimsIdentity();

                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
                identity.AddClaim(new Claim(ClaimTypes.Name, username));

                if (roles != null)
                {
                    roles.ForEach(r => identity.AddClaim(new Claim(ClaimTypes.Role, r)));
                }

                princibal.AddIdentity(identity);

                _currentUserPrincipal = princibal;
            }
        }

        public void Logout()
        {
            lock (_obj)
            {
                _currentUser = null;
                _currentUserPrincipal = null;
            }
        }

        public ApplicationUser GetCurrentUser() => _currentUser;
        public ClaimsPrincipal? GetCurrentUserPrincibal() => _currentUserPrincipal;
    }
}
