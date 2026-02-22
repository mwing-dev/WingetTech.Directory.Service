using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticsController : ControllerBase
    {
        private readonly IDirectoryService _directoryService;
        private readonly IAuthenticationProbe _authenticationProbe;

        public DiagnosticsController(IDirectoryService directoryService, IAuthenticationProbe authenticationProbe)
        {
            _directoryService = directoryService;
            _authenticationProbe = authenticationProbe;
        }

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
                var result = new TestBindResponseDto(
                    false,
                    ex.Message,
                    DateTime.UtcNow
                );

                return Ok(result);
            }
        }
    }
}
