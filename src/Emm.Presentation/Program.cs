using Emm.Application.Extensions;
using Emm.Infrastructure.Extensions;
using Emm.Presentation.Extensions;
using Emm.Presentation.Options;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers(options =>
{
    // Add global model state validation filter
    options.Filters.Add<Emm.Presentation.Filters.ValidateModelStateFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    // Disable automatic 400 response for model validation errors
    // We handle this in our custom filter instead
    options.SuppressModelStateInvalidFilter = true;
});

builder.AddObservability();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddSignalR();

// Configure error handling options
builder.Services.Configure<ErrorHandlingOptions>(
    builder.Configuration.GetSection(ErrorHandlingOptions.SectionName));

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // default format
builder.Services.AddSwaggerCustomGen();

// Add rate limiting
builder.Services.AddCustomRateLimiting();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});



var app = builder.Build();

// Seed database with initial data
// await app.SeedDatabaseAsync();

// Global exception handler - must be first in pipeline
app.UseGlobalExceptionHandler();

app.UseCors("AllowAll");
app.MapOpenApi();

// Use rate limiting (must be before Authentication/Authorization)
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Single device login middleware - phải đặt sau Authentication và Authorization
app.UseSingleDeviceLogin();

// Configure static files serving
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")),
    RequestPath = "/files"
});

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
