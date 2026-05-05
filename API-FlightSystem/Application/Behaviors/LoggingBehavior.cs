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

            Logging.Info("Handling {RequestName} {@Request}", requestName, request);

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await next();
                stopwatch.Stop();

                Logging.Info("Handled {RequestName} in {ElapsedMilliseconds}ms {@Response}",
                    requestName, stopwatch.ElapsedMilliseconds, response);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                Logging.Error(ex, "Error handling {RequestName} in {ElapsedMilliseconds}ms",
                    requestName, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}