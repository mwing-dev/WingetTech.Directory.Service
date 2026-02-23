using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class OrganizationalUnitsController : ControllerBase
    {
        private readonly IDirectoryService _directoryService;
        private readonly ILogger<OrganizationalUnitsController> _logger;

        public OrganizationalUnitsController(IDirectoryService directoryService, ILogger<OrganizationalUnitsController> logger)
        {
            _directoryService = directoryService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a single OrganizationalUnit matching the provided distinguished name.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(OrganizationalUnitDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrganizationalUnitDto>> GetOrganizationalUnit(
            [FromQuery] string distinguishedName,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(distinguishedName))
                return BadRequest(new { message = "A non-empty distinguishedName is required." });

            // Basic DN format validation: must contain at least one component separator
            if (!distinguishedName.Contains('='))
                return BadRequest(new { message = "Invalid distinguishedName format." });

            _logger.LogInformation("OU lookup for distinguished name {DistinguishedName}", distinguishedName);

            var ou = await _directoryService.GetOrganizationalUnitAsync(distinguishedName, cancellationToken);
            if (ou is null)
                return NotFound();

            return Ok(new OrganizationalUnitDto(
                ou.Name,
                ou.DistinguishedName,
                ou.ParentDn));
        }
    }
}
