using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers;

/// <summary>
/// Controller for diagnostics and health checks.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DiagnosticsController : ControllerBase
{
    private readonly IDirectoryService _directoryService;
    private readonly IAuthenticationProbe _authenticationProbe;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiagnosticsController"/> class.
    /// </summary>
    /// <param name="directoryService">The directory service.</param>
    /// <param name="authenticationProbe">The authentication probe.</param>
    public DiagnosticsController(IDirectoryService directoryService, IAuthenticationProbe authenticationProbe)
    {
        _directoryService = directoryService;
        _authenticationProbe = authenticationProbe;
    }

    /// <summary>
    /// Performs a health check on the directory service.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The health status of the directory service.</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(HealthCheckDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HealthCheckDto>> HealthCheck(CancellationToken cancellationToken)
    {
        var isHealthy = await _directoryService.HealthCheckAsync(cancellationToken);
        
        var result = new HealthCheckDto(
            isHealthy,
            isHealthy ? "Directory service is healthy" : "Directory service is unhealthy",
            DateTime.UtcNow
        );

        return Ok(result);
    }

    [HttpPost("test-bind")]
    [ProducesResponseType(typeof(TestBindResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TestBindResponseDto>> TestBind(
        CancellationToken cancellationToken)
    {
        try
        {
            var success = await _authenticationProbe.TestBindAsync(cancellationToken);

            var result = new TestBindResponseDto(
                success,
                success ? null : "LDAP bind failed.",
                DateTime.UtcNow
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Don't leak sensitive details
            var result = new TestBindResponseDto(
                false,
                ex.Message,
                DateTime.UtcNow
            );

            return Ok(result);
        }
    }
}
