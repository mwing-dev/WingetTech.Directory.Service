using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationalUnitsController : ControllerBase
{
    private readonly IDirectoryService _directoryService;

    public OrganizationalUnitsController(IDirectoryService directoryService)
    {
        _directoryService = directoryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(OrganizationalUnitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrganizationalUnitDto>> GetOrganizationalUnit(
        [FromQuery] string distinguishedName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(distinguishedName))
            return BadRequest("A non-empty distinguishedName is required.");

        var ou = await _directoryService.GetOrganizationalUnitAsync(distinguishedName, cancellationToken);
        if (ou is null)
            return NotFound();

        return Ok(new OrganizationalUnitDto(
            ou.Name,
            ou.DistinguishedName,
            ou.ParentDn));
    }
}