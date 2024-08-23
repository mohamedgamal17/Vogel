using Microsoft.AspNetCore.Mvc;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Shared.Results;

namespace Vogel.BuildingBlocks.Infrastructure.Extensions
{
    public static class EndpointResultsExtensions
    {
        public static IResult ToOk<T>(this Result<T> result)
        {
            if (result.IsFailure)
            {
                return HandleFailureResults(result);
            }

            return Results.Ok(result.Value);
        }

        public static IResult ToCreated<T>(this Result<T> result , string? uri = null)
        {
            if (result.IsFailure)
            {
                return HandleFailureResults(result);

            }

            return Results.Created(uri, result.Value);
        }

        public static IResult ToCreatedAtRoute<T>(this Result<T> result  , string? routeName = null , object? routeValues = null)
        {
            if (result.IsFailure)
            {
                return HandleFailureResults(result);

            }

            return Results.CreatedAtRoute(routeName: routeName, routeValues: routeValues, result.Value);
        }

        public static IResult ToNoContent<T>(this Result<T> result)
        {
            if (result.IsFailure)
            {
                return HandleFailureResults(result);
            }

            return Results.NoContent();
        }


        private static IResult HandleFailureResults<T>( Result<T> result)
        {
            Exception exception = result.Exception!;

            if (exception is ForbiddenAccessException)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
                };

                return Results.Problem(problemDetails);
            }
            else if (exception is UnauthorizedAccessException)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
                };

                return Results.Problem(problemDetails);
            }
            else if (exception is EntityNotFoundException notFoundException)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "The specified resource was not found.",
                    Detail = notFoundException.Message
                };

                return Results.Problem(problemDetails);
            }
            else if (exception is BusinessLogicException businessLogicException)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Invalid entity state.",
                    Detail = businessLogicException.Message
                };

                return Results.Problem(problemDetails);
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Internal server error",
                    Detail = exception.Message
                };


                return Results.Problem(problemDetails);
            }
        }

    }
}
