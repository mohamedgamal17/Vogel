using System.Security.Claims;

namespace Vogel.BuildingBlocks.Infrastructure.Security
{
    public interface ISecurityContext
    {
        bool IsUserAuthenticated { get; }
        ApplicationUser? User { get; }
        ClaimsPrincipal? UserClaimsPrincipal { get; }
    }
}
