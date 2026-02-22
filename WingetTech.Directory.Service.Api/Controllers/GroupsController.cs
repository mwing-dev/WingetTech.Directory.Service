using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IDirectoryService _directoryService;

    public GroupsController(IDirectoryService directoryService)
    {
        _directoryService = directoryService;
    }

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
            group.Members
        );

        return Ok(groupDto);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(GroupSearchResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<GroupSearchResultDto>> SearchGroups([FromQuery] string filter, CancellationToken cancellationToken)
    {
        var groups = await _directoryService.SearchGroupsAsync(filter, cancellationToken);
        
        var groupDtos = groups.Select(g => new GroupDto(
            g.Id,
            g.Name,
            g.DistinguishedName,
            g.Members
        )).ToList();

        var result = new GroupSearchResultDto(groupDtos, groupDtos.Count);
        return Ok(result);
    }
}
