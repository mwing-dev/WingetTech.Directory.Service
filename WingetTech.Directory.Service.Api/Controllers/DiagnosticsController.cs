using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DiagnosticsController : ControllerBase
    {
        private readonly IAuthenticationProbe _authenticationProbe;
        private readonly ILogger<DiagnosticsController> _logger;

        public DiagnosticsController(IAuthenticationProbe authenticationProbe, ILogger<DiagnosticsController> logger)
        {
            _authenticationProbe = authenticationProbe;
            _logger = logger;
        }

        [HttpPost("test-bind")]
        [ProducesResponseType(typeof(TestBindResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<TestBindResponseDto>> TestBind(CancellationToken cancellationToken)
        {
            try
            {
                var success = await _authenticationProbe.TestBindAsync(cancellationToken);

                if (!success)
                {
                    _logger.LogWarning("LDAP test-bind failed");
                }

                var result = new TestBindResponseDto(
                    success,
                    success ? null : "Directory bind failed. Check server logs for details.",
                    DateTime.UtcNow);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LDAP test-bind threw an exception");

                var result = new TestBindResponseDto(
                    false,
                    "Directory bind failed. Check server logs for details.",
                    DateTime.UtcNow);

                return Ok(result);
            }
        }
    }
}
