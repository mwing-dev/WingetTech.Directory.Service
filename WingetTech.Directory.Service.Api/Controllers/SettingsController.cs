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

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsController"/> class.
    /// </summary>
    /// <param name="settingsService">The directory settings service.</param>
    public SettingsController(IDirectorySettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <summary>
    /// Saves or updates the directory connection settings.
    /// </summary>
    /// <param name="dto">The directory settings to save.</param>
    /// <returns>200 OK on success.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Save([FromBody] DirectorySettingsDto dto)
    {
        await _settingsService.SaveAsync(dto);
        return Ok();
    }

    /// <summary>
    /// Returns the current directory connection settings.
    /// </summary>
    /// <returns>The directory settings, or 404 if none have been configured.</returns>
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
