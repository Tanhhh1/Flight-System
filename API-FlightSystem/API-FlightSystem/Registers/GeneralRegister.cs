using API_FlightBooking.Configurations;
using Application.Common;
using Application.Hubs;
using Asp.Versioning;
using Domain.Identity;
using Infrastructure.Persistences;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Shared.Identity;
using System.Text;
using System.Text.Json;

namespace API_FlightBooking.Registers
{
    public static class GeneralRegister
    {
        private static readonly string PolicyName = "ApiFlightSystem";

        public static void RegisterGeneralServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigOption(configuration);
            services.VersionApiInjection();
            services.IdentityInjection();
            services.JwtInjection(configuration);
        }

        public static void RegisterGeneralApp(this WebApplication app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseRouting();
            app.UseCors(PolicyName);
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.MapControllers();
            app.MapHub<SeatHub>("/hubs/seat");
        }

        private static void VersionApiInjection(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
        }

        private static void IdentityInjection(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>
                (options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 1;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = true;

                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                })
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();
        }

        private static void JwtInjection(this IServiceCollection services, IConfiguration configuration)
        {
            var secretKey = configuration.GetValue<string>("JwtConfiguration:SecretKey");
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("JwtConfiguration:SecretKey is missing. Add 'JwtConfiguration:SecretKey' to appsettings.json or provide it via environment variables.");
            }

            services.Configure<JwtSetting>(configuration.GetSection("JwtConfiguration"));
            var key = Encoding.UTF8.GetBytes(secretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/seat"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        var message = context.AuthenticateFailure?.GetType().Name switch
                        {
                            "SecurityTokenExpiredException" => "Token đã hết hạn, vui lòng đăng nhập lại",
                            "SecurityTokenInvalidSignatureException" => "Token không hợp lệ",
                            "SecurityTokenNotFoundException" => "Không tìm thấy token",
                            _ => "Bạn chưa đăng nhập"
                        };

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = ApiResult<object>.Failure(message);
                        await context.Response.WriteAsync(
                            JsonSerializer.Serialize(response, new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                            })
                        );
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = ApiResult<object>.Failure("Bạn không có quyền thực hiện chức năng này");
                        await context.Response.WriteAsync(
                            JsonSerializer.Serialize(response, new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                            })
                        );
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Auth failed: {context.Exception.GetType().Name}: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                };
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtConfiguration:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtConfiguration:ValidAudience"],
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        private static void ConfigOption(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                var withOrigins = configuration.GetSection("ServerSetting:WithOrigins").Get<string[]>();

                if (withOrigins == null || withOrigins.Length == 0)
                {
                    var singleOrigin = configuration.GetValue<string>("ServerSetting:WithOrigins");
                    if (!string.IsNullOrEmpty(singleOrigin))
                    {
                        withOrigins = [singleOrigin];
                    }
                }
                withOrigins ??= ["http://localhost:5173"];
                options.AddPolicy(PolicyName, policy =>
                {
                    policy.WithOrigins(withOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            services.Configure<FormOptions>(x =>
            {
                x.ValueCountLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = 83886080;
                x.MultipartHeadersLengthLimit = 83886080;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue;
            });
            services.AddSignalR(
                    hubOptions =>
                    {
                        hubOptions.EnableDetailedErrors = true;
                        hubOptions.MaximumReceiveMessageSize = null;
                    })
            .AddNewtonsoftJsonProtocol(
                    opt =>
                        opt.PayloadSerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }
    }
}
