using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers;

/// <summary>
/// Controller for managing directory groups.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IDirectoryService _directoryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupsController"/> class.
    /// </summary>
    /// <param name="directoryService">The directory service.</param>
    public GroupsController(IDirectoryService directoryService)
    {
        _directoryService = directoryService;
    }

    /// <summary>
    /// Gets a group by its name or identifier.
    /// </summary>
    /// <param name="identifier">The name or identifier of the group.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The group if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("{identifier}")]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupDto>> GetGroup(string identifier, CancellationToken cancellationToken)
    {
        var group = await _directoryService.GetGroupAsync(identifier, cancellationToken);
        if (group == null)
        {
            return NotFound();
        }

        var groupDto = new GroupDto(
            group.Id,
            group.Name,
            group.DistinguishedName,
            group.Members,
            group.Description
        );

        return Ok(groupDto);
    }

    /// <summary>
    /// Searches for groups matching the specified filter.
    /// </summary>
    /// <param name="filter">The search filter to apply.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A collection of groups matching the search criteria.</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(GroupSearchResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GroupSearchResultDto>> SearchGroups([FromQuery] string filter, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return BadRequest("A non-empty filter is required.");

        var groups = await _directoryService.SearchGroupsAsync(filter, cancellationToken);
        
        var groupDtos = groups.Select(g => new GroupDto(
            g.Id,
            g.Name,
            g.DistinguishedName,
            g.Members,
            g.Description
        )).ToList();

        var result = new GroupSearchResultDto(groupDtos, groupDtos.Count);
        return Ok(result);
    }
}
