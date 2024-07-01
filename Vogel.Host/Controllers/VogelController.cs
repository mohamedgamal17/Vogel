﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vogel.Application.Common.Models;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Host.Models;

namespace Vogel.Host.Controllers
{
    public abstract class VogelController : Controller
    {
        public IServiceProvider ServiceProvider { get;  }
        public IMediator Mediator { get;  }
        public VogelController(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Mediator = ServiceProvider.GetRequiredService<IMediator>();
        }
        protected async Task<Result<T>> SendAsync<T>(IApplicationReuest<T> request)
        {
            return await Mediator.Send(request);
        }
        public IActionResult Ok<T>(Result<Paging<T>> result)
        {
            if (result.IsSuccess)
            {
                var apiResponse = new ApiResponse<List<T>>() 
                { 
                    Data = result.Value!.Data ,
                    PagingInfo = result.Value!.Info
                };

                return base.Ok(apiResponse);
            }

            return HandleFailureResult(result);
        }


        public IActionResult Ok<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                var apiResponse = new ApiResponse<T>() { Data = result.Value! };

                return base.Ok(apiResponse);
            }

            return HandleFailureResult(result);
        }

        public IActionResult CreatedAtAction<T>(Result<T> result , string actionName ,object? routeValues = null)
        {
            if (result.IsSuccess)
            {
                var apiResponse = new ApiResponse<T>() { Data = result.Value! };

                return base.CreatedAtAction(actionName, routeValues, apiResponse);
            }

            return HandleFailureResult(result);
        }

        public IActionResult NoContent<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return HandleFailureResult(result);
        }


        private IActionResult HandleFailureResult<T>(Result<T> result)
        {
            Exception exception = result.Exception!;

            if(exception is ForbiddenAccessException)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
                };

                return StatusCode(StatusCodes.Status403Forbidden, problemDetails);
            }
            else if(exception is UnauthorizedAccessException)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
                };

                return StatusCode(StatusCodes.Status401Unauthorized, problemDetails);
            }
            else if(exception is EntityNotFoundException notFoundException)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "The specified resource was not found.",
                    Detail = notFoundException.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, problemDetails);
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

                return StatusCode(StatusCodes.Status400BadRequest, problemDetails);
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


                return StatusCode(StatusCodes.Status500InternalServerError , problemDetails);
            }
        }

    }
}
