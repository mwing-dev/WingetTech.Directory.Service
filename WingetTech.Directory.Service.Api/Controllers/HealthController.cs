using Microsoft.AspNetCore.Mvc;

namespace WingetTech.Directory.Service.Api.Controllers;

/// <summary>
/// Health check controller for service monitoring.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Returns the health status of the service.
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "WingetTech.Directory.Service"
        });
    }
}
