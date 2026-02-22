using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers;

/// <summary>
/// Controller for managing AD/LDAP connection settings.
/// </summary>
[ApiController]
[Route("api/settings/directory")]
public class SettingsController : ControllerBase
{
    private readonly IDirectorySettingsService _settingsService;

    public SettingsController(IDirectorySettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Save([FromBody] DirectorySettingsDto dto)
    {
        await _settingsService.SaveAsync(dto);
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

        return Ok(settings);
    }
}
