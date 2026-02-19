using Scalar.AspNetCore;
using WingetTech.Directory.Service.Core;
using WingetTech.Directory.Service.Core.Configuration;
using WingetTech.Directory.Service.Core.Interfaces;
using WingetTech.Directory.Service.Infrastructure;

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

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapOpenApi().AllowAnonymous();
app.MapScalarApiReference().AllowAnonymous();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
