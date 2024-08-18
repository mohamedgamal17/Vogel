using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Shared.Results;
namespace Vogel.BuildingBlocks.Application.Behaviours
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IApplicationPiplineBehaviour<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ISecurityContext _securityContext;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public AuthorizationBehaviour(ISecurityContext securityContext, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _securityContext = securityContext;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

            if (authorizeAttributes.Any())
            {
                if (!_securityContext.IsUserAuthenticated)
                {
                    return new Result<TResponse>(new UnauthorizedAccessException());
                }

                var currentUser = _securityContext.User!;


                //Role based authorization

                var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

                if (authorizeAttributesWithRoles.Any())
                {
                    bool authorized = false;

                    foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles!.Split(',')))
                    {
                        foreach (var role in roles)
                        {
                            bool isInRole = currentUser.Roles.Contains(role);
                            if (isInRole)
                            {
                                authorized = true;
                                break;
                            }
                        }
                    }

                    if (!authorized)
                    {
                        return new Result<TResponse>(new ForbiddenAccessException());
                    }
                }

                // Policy-based authorization
                var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));

                if (authorizeAttributesWithPolicies.Any())
                {
                    foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                    {
                        var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(policy!);

                        if (authorizationResult.IsFailure)
                        {
                            return new Result<TResponse>(authorizationResult.Exception!);
                        }
                    }
                }
            }

            return await next();
        }
    }
}
