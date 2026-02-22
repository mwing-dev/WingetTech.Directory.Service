using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WingetTech.Directory.Service.Core.Configuration;
using WingetTech.Directory.Service.Core.Interfaces;
using WingetTech.Directory.Service.Infrastructure;
using WingetTech.Directory.Service.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); // required for controllers
builder.Services.AddOpenApi();              // generates the OpenAPI document

// Register directory service
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IDirectoryService, LdapDirectoryService>();
builder.Services.AddScoped<IAuthenticationProbe, LdapAuthenticationProbe>();
builder.Services.AddScoped<IDirectorySettingsService, DirectorySettingsService>();
builder.Services.AddScoped<IEncryptionService, PlainTextEncryptionService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure EF Core with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=directory.db"));

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapOpenApi().AllowAnonymous();
app.MapScalarApiReference().AllowAnonymous();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
