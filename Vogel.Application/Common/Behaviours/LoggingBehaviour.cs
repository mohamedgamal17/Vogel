using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
namespace Vogel.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
    {
        private readonly ILogger<TRequest> _logger;

        public LoggingBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public  Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Processing Vogel Application Request: {Name}  {@Request}",
                requestName,  request);
            }

            return Task.CompletedTask;
        }
    }
}
