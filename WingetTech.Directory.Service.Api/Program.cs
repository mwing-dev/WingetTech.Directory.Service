using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WingetTech.Directory.Service.Core;
using WingetTech.Directory.Service.Core.Configuration;
using WingetTech.Directory.Service.Core.Interfaces;
using WingetTech.Directory.Service.Infrastructure;
using WingetTech.Directory.Service.Infrastructure.Persistence;
using WingetTech.Directory.Service.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); // required for controllers
builder.Services.AddOpenApi();              // generates the OpenAPI document

// Configure strongly-typed settings
builder.Services.Configure<DirectoryOptions>(
    builder.Configuration.GetSection("Directory"));

// Register directory service
builder.Services.AddScoped<IDirectoryService, LdapDirectoryService>();

// Register database context
builder.Services.AddDbContext<DirectoryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DirectoryDb") ?? "Data Source=directory.db"));

// Register database settings services
builder.Services.AddScoped<IEncryptionService, PlainTextEncryptionService>();
builder.Services.AddScoped<IDatabaseSettingsService, DatabaseSettingsService>();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Ensure the database schema is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DirectoryDbContext>();
    db.Database.EnsureCreated();
}

app.MapOpenApi().AllowAnonymous();
app.MapScalarApiReference().AllowAnonymous();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
