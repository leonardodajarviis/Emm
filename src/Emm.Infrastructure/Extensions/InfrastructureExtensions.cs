using System.Text;
using Emm.Application.Abstractions;
using Emm.Application.Abstractions.Security;
using Emm.Domain.Abstractions;
using Emm.Domain.Repositories;
using Emm.Domain.Services;
using Emm.Infrastructure.Data;
using Emm.Infrastructure.Data.Interceptors;
using Emm.Infrastructure.Messaging;
using Emm.Infrastructure.Options;
using Emm.Infrastructure.Repositories;
using Emm.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace Emm.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Interceptors - must be registered before DbContext
        services.AddSingleton<DomainEventInterceptor>();

        services.AddDbContext<XDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.UseLoggerFactory(NullLoggerFactory.Instance);   // Disable logging
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        });


        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEventSerializer, SystemTextJsonEventSerializer>();

        // Register repositories
        services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));
        services.AddScoped<IQueryContext, QueryContext>();
        services.AddScoped<IAssetModelRepository, AssetModelRepository>();
        services.AddScoped<IAssetTypeRepository, AssetTypeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<IOperationShiftRepository, OperationShiftRepository>();
        services.AddScoped<IShiftLogRepository, ShiftLogRepository>();
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IMaintenancePlanDefinitionRepository, MaintenancePlanDefinitionRepository>();
        services.AddScoped<ICodeGenerator, SequenceCodeGenerator>();

        // Authorization repositories
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();

        // Authorization services
        services.AddScoped<IPolicyEvaluator, PolicyEvaluator>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddMemoryCache(); // For authorization caching

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Register DateTimeProvider
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Register HttpContextAccessor and UserContextService
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContextService, UserContextService>();

        // Register Authentication Settings
        services.AddSingleton<IAuthenticationSettings, AuthenticationSettings>();

        // Register Outbox
        services.AddScoped<IOutbox, Outbox>();

        // Register File Storage
        services.Configure<LocalFileStorageOptions>(configuration.GetSection(LocalFileStorageOptions.SectionName));
        services.AddScoped<IFileStorage, LocalFileStorage>();

        // Register Security
        services.AddScoped<ISecurityService, Security.SecurityService>();

        // Note: Register all Policies automatically if needed
        // services.Scan(scan => scan
        //     .FromAssemblies(typeof(Application.Abstractions.Security.IResourcePolicy<>).Assembly)
        //     .AddClasses(classes => classes.AssignableTo(typeof(Application.Abstractions.Security.IResourcePolicy<>)))
        //     .AsImplementedInterfaces()
        //     .WithScopedLifetime());

        services.Configure<OutboxProcessorOptions>(cfg =>
        {
            cfg.BatchSize = 50;
            cfg.MaxAttempts = 10;
            cfg.LockDuration = TimeSpan.FromMinutes(1);
            cfg.IdleDelay = TimeSpan.FromMilliseconds(500);
        });

        services.AddHostedService<OutboxProcessor>();
        services.AddJwtAuthentication(configuration);

        return services;
    }
    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        var jwtOptions = jwtSection.Get<JwtOptions>();
        services.Configure<JwtOptions>(jwtSection);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions?.AccessKey ?? string.Empty)),
                ClockSkew = TimeSpan.FromSeconds(30) // Allow 30 seconds clock skew tolerance
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notification"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    // Skip the default behavior
                    context.HandleResponse();

                    // Set response status code
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";

                    // Create custom error response
                    var errorResponse = new
                    {
                        code = "AUTHENTICATION_REQUIRED",
                        message = "Authentication token is missing or invalid"
                    };

                    return context.Response.WriteAsJsonAsync(errorResponse);
                }
            };
        });

        services.AddSingleton<IJwtService, JwtService>();

        return services;
    }
}
