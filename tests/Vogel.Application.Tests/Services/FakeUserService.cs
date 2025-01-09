using NUnit.Framework;
using System.Security.Claims;
using Vogel.BuildingBlocks.Infrastructure.Security;

namespace Vogel.Application.Tests.Services
{
    public class FakeUserService
    {
        private AsyncLocal<ClaimsPrincipal?> _ambientUserPrincipal = new AsyncLocal<ClaimsPrincipal?>();
        public bool IsAuthenticated => _ambientUserPrincipal.Value != null;
        public void Login()
        {
            Login(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), new List<string>());
        }

        public void Login(string id, string username, List<string> roles)
        {

            var princibal = new ClaimsPrincipal();

            var identity = new ClaimsIdentity("TEST_AUTHENTICATION") ;

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
            identity.AddClaim(new Claim(ClaimTypes.Name, username));

            if (roles != null)
            {
                roles.ForEach(r => identity.AddClaim(new Claim(ClaimTypes.Role, r)));
            }

            princibal.AddIdentity(identity);

            _ambientUserPrincipal.Value = princibal;

        }

        public void Logout()
        {

            _ambientUserPrincipal.Value = null;

        }

        public ApplicationUser? GetCurrentUser()
        {
            var princibal = _ambientUserPrincipal.Value;

            if (princibal != null)
            {
                var user = new ApplicationUser
                {
                    Id = princibal.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value,
                    UserName = princibal.Claims.Single(x => x.Type == ClaimTypes.Name).Value,
                    Roles = princibal.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList()
                };

                return user;
            }

            return null;
        }
        public ClaimsPrincipal? GetCurrentUserPrincibal() => _ambientUserPrincipal.Value;
    }
}
