using Microsoft.OpenApi.Models;

namespace Emm.Presentation.Extensions;

public static class SwaggerGenExtensions
{
    public static IServiceCollection AddSwaggerCustomGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "XEmm API",
                Description = "An ASP.NET Core Web API for managing XEmm",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Contact Jarviisha",
                    Url = new Uri("https://jarviisha.com")
                },
                License = new OpenApiLicense
                {
                    Name = "License",
                    Url = new Uri("https://jarviisha.com/license")
                }
            });

            // Add JWT Authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });
        services.AddOpenApi();

        return services;
    }
}
