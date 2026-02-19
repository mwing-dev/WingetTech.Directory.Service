using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers;

/// <summary>
/// Controller for managing organizational units.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrganizationalUnitsController : ControllerBase
{
    private readonly IDirectoryService _directoryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationalUnitsController"/> class.
    /// </summary>
    /// <param name="directoryService">The directory service.</param>
    public OrganizationalUnitsController(IDirectoryService directoryService)
    {
        _directoryService = directoryService;
    }

    /// <summary>
    /// Gets an organizational unit by its distinguished name.
    /// </summary>
    /// <param name="distinguishedName">The distinguished name of the organizational unit.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The organizational unit if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(OrganizationalUnitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrganizationalUnitDto>> GetOrganizationalUnit([FromQuery] string distinguishedName, CancellationToken cancellationToken)
    {
        var ou = await _directoryService.GetOrganizationalUnitAsync(distinguishedName, cancellationToken);
        if (ou == null)
        {
            return NotFound();
        }

        var ouDto = new OrganizationalUnitDto(
            ou.Name,
            ou.DistinguishedName,
            ou.ParentDn
        );

        return Ok(ouDto);
    }
}
