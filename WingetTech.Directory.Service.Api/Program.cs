using WingetTech.Directory.Service.Core;
using WingetTech.Directory.Service.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Windows Service
// TODO: Uncomment when Microsoft.Extensions.Hosting.WindowsServices is available for net10.0
// builder.Host.UseWindowsService();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
// Configure strongly-typed settings
builder.Services.Configure<DirectoryOptions>(
    builder.Configuration.GetSection("Directory"));

// Register directory service
builder.Services.AddScoped<IDirectoryService, LdapDirectoryService>();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapOpenApi().AllowAnonymous();
app.MapScalarApiReference().AllowAnonymous();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
