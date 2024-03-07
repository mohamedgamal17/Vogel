using System.Security.Claims;
using Vogel.Application.Common.Models;

namespace Vogel.Application.Common.Interfaces
{
    public interface ISecurityContext
    {
        public bool IsUserAuthenticated { get; }
        public ApplicationUser? User { get; }

        public ClaimsPrincipal? UserClaimsPrincipal { get; }
    }
}
