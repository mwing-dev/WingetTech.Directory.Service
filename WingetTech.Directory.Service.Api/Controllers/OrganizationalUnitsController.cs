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
