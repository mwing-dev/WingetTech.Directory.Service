using WingetTech.Directory.Service.Core.Configuration;
using WingetTech.Directory.Service.Core.Interfaces;
using WingetTech.Directory.Service.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure Windows Service
// TODO: Uncomment when Microsoft.Extensions.Hosting.WindowsServices is available for net10.0
// builder.Host.UseWindowsService();

// Add services to the container
builder.Services.AddControllers();

// Configure strongly-typed settings
builder.Services.Configure<DirectoryOptions>(
    builder.Configuration.GetSection("Directory"));

// Register directory service
builder.Services.AddScoped<IDirectoryService, LdapDirectoryService>();
builder.Services.AddScoped<IAuthenticationProbe, LdapAuthenticationProbe>();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
