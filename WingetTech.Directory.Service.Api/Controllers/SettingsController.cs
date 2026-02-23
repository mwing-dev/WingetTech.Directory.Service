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
        private readonly IDirectorySettingsService _settingsService;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(IDirectorySettingsService settingsService, ILogger<SettingsController> logger)
        {
            _settingsService = settingsService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Save([FromBody] DirectorySettingsDto dto)
        {
            await _settingsService.SaveAsync(dto);
            _logger.LogInformation("Directory settings updated by {User}", User.Identity?.Name);
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(DirectorySettingsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DirectorySettingsDto>> Get()
        {
            var settings = await _settingsService.GetAsync();

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
