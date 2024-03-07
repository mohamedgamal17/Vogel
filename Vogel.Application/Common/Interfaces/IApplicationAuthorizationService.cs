using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Vogel.Domain.Utils;

namespace Vogel.Application.Common.Interfaces
{
    public interface IApplicationAuthorizationService
    {
        Task<Result<Unit>> AuthorizeAsync(object resource, IAuthorizationRequirement requirements);
        Task<Result<Unit>> AuthorizeAsync(object resource, string policyName);
        Task<Result<Unit>> AuthorizeAsync(string policyName);
    }
}
