using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers
{
    /// <summary>
    /// Readiness health check: returns application health and directory connectivity status.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IDirectoryService _directoryService;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IDirectoryService directoryService, ILogger<HealthController> logger)
        {
            _directoryService = directoryService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(HealthCheckDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HealthCheckDto), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<HealthCheckDto>> Get(CancellationToken cancellationToken)
        {
            var directoryHealthy = await _directoryService.HealthCheckAsync(cancellationToken);

            var result = new HealthCheckDto(
                directoryHealthy,
                directoryHealthy ? "Service is healthy" : "Directory connectivity is unavailable",
                DateTime.UtcNow);

            if (!directoryHealthy)
            {
                _logger.LogWarning("Health check reports directory connectivity is unavailable");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, result);
            }

            return Ok(result);
        }
    }
}
