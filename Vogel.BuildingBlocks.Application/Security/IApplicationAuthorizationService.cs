using MediatR;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Domain.Results;
namespace Vogel.BuildingBlocks.Application.Security
{
    public interface IApplicationAuthorizationService
    {
        Task<Result<Unit>> AuthorizeAsync(object resource, IAuthorizationRequirement requirements);
        Task<Result<Unit>> AuthorizeAsync(object resource, string policyName);
        Task<Result<Unit>> AuthorizeAsync(string policyName);
    }
}
