using MediatR;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Shared.Results;
namespace Vogel.BuildingBlocks.Infrastructure.Security
{
    public interface IApplicationAuthorizationService
    {
        Task<Result<Unit>> AuthorizeAsync(IAuthorizationRequirement requirement);
        Task<Result<Unit>> AuthorizeAsync(object resource, IAuthorizationRequirement requirements);
        Task<Result<Unit>> AuthorizeAsync(object resource, string policyName);
        Task<Result<Unit>> AuthorizeAsync(string policyName);
    }
}
