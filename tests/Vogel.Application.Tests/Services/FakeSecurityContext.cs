using System.Security.Claims;
using Vogel.BuildingBlocks.Infrastructure.Security;

namespace Vogel.Application.Tests.Services
{
    public class FakeSecurityContext : ISecurityContext
    {

        private readonly FakeUserService _userService;

        public FakeSecurityContext(FakeUserService userService)
        {
            _userService = userService;
        }

        public bool IsUserAuthenticated => _userService.GetCurrentUser() != null;

        public ApplicationUser? User => _userService.GetCurrentUser();

        public ClaimsPrincipal? UserClaimsPrincipal => _userService.GetCurrentUserPrincibal();
    }
}
