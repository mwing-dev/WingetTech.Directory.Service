using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers;

/// <summary>
/// Controller for managing database connection settings.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DatabaseSettingsController : ControllerBase
{
    private readonly IDatabaseSettingsService _databaseSettingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseSettingsController"/> class.
    /// </summary>
    /// <param name="databaseSettingsService">The database settings service.</param>
    public DatabaseSettingsController(IDatabaseSettingsService databaseSettingsService)
    {
        _databaseSettingsService = databaseSettingsService;
    }

    /// <summary>
    /// Saves (creates or updates) the database connection settings.
    /// </summary>
    /// <param name="dto">The database settings to save.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Save([FromBody] DatabaseSettingsDto dto, CancellationToken cancellationToken)
    {
        await _databaseSettingsService.SaveAsync(dto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Retrieves the current database connection settings.
    /// The password field is masked in the response.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The current database settings, or 404 if not configured.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(DatabaseSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DatabaseSettingsDto>> Get(CancellationToken cancellationToken)
    {
        var settings = await _databaseSettingsService.GetAsync(cancellationToken);

        if (settings is null)
            return NotFound();

        return Ok(settings with { Password = "****" });
    }
}
