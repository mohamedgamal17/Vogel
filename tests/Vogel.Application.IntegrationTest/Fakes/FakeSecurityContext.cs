using System.Security.Claims;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;
using Vogel.BuildingBlocks.Application.Security;

namespace Vogel.Application.IntegrationTest.Fakes
{
    public class FakeSecurityContext : ISecurityContext
    {
        public bool IsUserAuthenticated => User != null;

        public ApplicationUser? User => TryToGetCurrentUser();

        private ApplicationUser? TryToGetCurrentUser()
        {
            var currentUser = Testing.CurrentUser;

            if (currentUser != null)
            {
                string userId = currentUser.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
                string? userName = currentUser.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                List<string> userRoles = currentUser.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value)
                    .ToList();
                return new ApplicationUser
                {
                    Id = userId,
                    UserName = userName,
                    Roles = userRoles
                };
            }

            return null;
        }
        public ClaimsPrincipal? UserClaimsPrincipal => Testing.CurrentUser;
    }
}
