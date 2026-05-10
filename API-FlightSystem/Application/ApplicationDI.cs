using Application.Behaviors;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationDI
    {
        public static void AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ApplicationDI).Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidateBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(ApplicationDI).Assembly);

            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(ApplicationDI).Assembly);
            services.AddSingleton(config);
            services.AddMapster();
        }
    }
}
