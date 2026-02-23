using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IDirectoryService _directoryService;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(IDirectoryService directoryService, ILogger<GroupsController> logger)
        {
            _directoryService = directoryService;
            _logger = logger;
        }

        [HttpGet("{identifier}")]
        [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GroupDto>> GetGroup(string identifier, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Group lookup for identifier {Identifier}", identifier);

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

        [HttpGet("search")]
        [ProducesResponseType(typeof(GroupSearchResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GroupSearchResultDto>> SearchGroups(
            [FromQuery] string filter,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return BadRequest(new { message = "A non-empty filter is required." });

            if (filter.Length < 2)
                return BadRequest(new { message = "Filter must be at least 2 characters." });

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 25;

            _logger.LogInformation("Group search with filter {Filter}, page {Page}, pageSize {PageSize}", filter, page, pageSize);

            var groups = await _directoryService.SearchGroupsAsync(filter, cancellationToken);

            var groupDtos = groups.Select(g => new GroupDto(
                g.Id,
                g.Name,
                g.DistinguishedName,
                g.Members,
                g.Description
            )).ToList();

            var totalCount = groupDtos.Count;
            var paged = groupDtos.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var result = new GroupSearchResultDto(paged, totalCount);
            return Ok(result);
        }
    }
}
