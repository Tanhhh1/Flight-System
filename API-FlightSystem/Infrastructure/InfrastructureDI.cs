using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Application.Services;
using Infrastructure.Caching;
using Infrastructure.Database;
using Infrastructure.Persistences;
using Infrastructure.Services;
using Infrastructure.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("Database");
            services.Configure<DatabaseConfiguration>(section);
            var databaseConfig = section.Get<DatabaseConfiguration>();
            if (databaseConfig == null) throw new Exception("Database configuration not found! Please check 'appsettings.json' file again.");
            services
                .AddDbContext<DatabaseContext>(options =>
                    options.UseNpgsql(
                        databaseConfig.Main,
                        opt => opt.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPaymentGateway, VNPayGateway>();
            services.AddSingleton<IMemoryCacheService, MemoryCacheService>();
            services.AddHostedService<FlightStatusUpdateService>();
            services.AddHostedService<SeatLockExpiryService>();

            return services;
        }
    }
}
