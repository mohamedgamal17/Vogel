using Microsoft.AspNetCore.Http;
using System.Security.Claims;
namespace Vogel.BuildingBlocks.Infrastructure.Security
{
    public class SecurityContext : ISecurityContext
    {
        private readonly HttpContext _httpContext;
        public bool IsUserAuthenticated => User != null;
        public ApplicationUser? User => TryToGetCurrentUser();
        public ClaimsPrincipal? UserClaimsPrincipal => GetUserClaimsPrincipal();

        public SecurityContext(IHttpContextAccessor _httpContextAccessor)
        {
            _httpContext = _httpContextAccessor.HttpContext!;
        }
        private ApplicationUser? TryToGetCurrentUser()
        {

            if (_httpContext.User.Identity?.IsAuthenticated ?? false)
            {
                string userId = _httpContext.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
                string? userName = _httpContext.User.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                List<string> userRoles = _httpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value)
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

        private ClaimsPrincipal? GetUserClaimsPrincipal()
        {
            return _httpContext.User;
        }
    }
}
