using Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Logger;
using System.Diagnostics;

namespace Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation("Handling {RequestName}", requestName);

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await next();
                stopwatch.Stop();

                var succeeded = response is ApiResult<object> result ? result.Succeeded : true;
                _logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds}ms - Succeeded: {Succeeded}",
                    requestName, stopwatch.ElapsedMilliseconds, succeeded);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "Error handling {RequestName} in {ElapsedMilliseconds}ms",
                     requestName, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}