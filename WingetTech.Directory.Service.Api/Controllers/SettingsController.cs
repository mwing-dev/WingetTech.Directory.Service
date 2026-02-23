using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/settings/directory")]
    public class SettingsController : ControllerBase
    {
        // Serializes concurrent bootstrap attempts so the auth-vs-no-settings check is atomic.
        private static readonly SemaphoreSlim _bootstrapLock = new(1, 1);

        private readonly IDirectorySettingsService _settingsService;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(IDirectorySettingsService settingsService, ILogger<SettingsController> logger)
        {
            _settingsService = settingsService;
            _logger = logger;
        }

        /// <summary>
        /// Saves directory settings. On a fresh install (no settings exist) this endpoint is
        /// accessible without authentication to allow first-run bootstrap. Once settings are
        /// stored, subsequent calls require a valid authorization token.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Save([FromBody] DirectorySettingsDto dto, CancellationToken cancellationToken)
        {
            await _bootstrapLock.WaitAsync(cancellationToken);
            try
            {
                var hasExistingSettings = await _settingsService.HasSettingsAsync(cancellationToken);

                // After the first save, require an authenticated caller
                if (hasExistingSettings && User.Identity?.IsAuthenticated != true)
                    return Unauthorized(new { message = "Authentication is required to update directory settings." });

                await _settingsService.SaveAsync(dto, cancellationToken);

                if (hasExistingSettings)
                {
                    _logger.LogInformation("Directory settings updated by {User}", User.Identity?.Name);
                }
                else
                {
                    _logger.LogWarning("Initial bootstrap: directory settings configured without authentication");
                }
            }
            finally
            {
                _bootstrapLock.Release();
            }

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(DirectorySettingsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DirectorySettingsDto>> Get(CancellationToken cancellationToken)
        {
            var settings = await _settingsService.GetAsync(cancellationToken);

            if (settings is null)
                return NotFound();

            // Never return the bind password in plaintext
            return Ok(new DirectorySettingsDto
            {
                Host = settings.Host,
                Port = settings.Port,
                UseSsl = settings.UseSsl,
                Domain = settings.Domain,
                BaseDn = settings.BaseDn,
                BindUsername = settings.BindUsername,
                BindPassword = "[REDACTED]"
            });
        }
    }
}
