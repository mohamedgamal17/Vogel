using MediatR;
using Microsoft.Extensions.Logging;
using Vogel.Application.Common.Interfaces;
using Vogel.Domain.Utils;
namespace Vogel.Application.Common.Behaviours
{
    public class ExceptionHandlingBehaviour<TRequest, TResponse> : IApplicationPiplineBehaviour<TRequest, TResponse>
        where TRequest : notnull      
    {
        private readonly ILogger<TRequest> _logger;

        public ExceptionHandlingBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();

            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;

                _logger.LogError(ex, "Vogel Application Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);

                 return new Result<TResponse>(ex);
            }
        }



    }
}
