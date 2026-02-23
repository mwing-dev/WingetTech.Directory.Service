using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IDirectoryService _directoryService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IDirectoryService directoryService, ILogger<UsersController> logger)
        {
            _directoryService = directoryService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserById(string id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User lookup by ID {Id}", id);

            var user = await _directoryService.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(MapToUserDto(user));
        }

        [HttpGet("by-username/{username}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserByUsername(string username, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User lookup by username {Username}", username);

            var user = await _directoryService.GetUserByUsernameAsync(username, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(MapToUserDto(user));
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(UserSearchResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserSearchResultDto>> SearchUsers(
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

            _logger.LogInformation("User search with filter {Filter}, page {Page}, pageSize {PageSize}", filter, page, pageSize);

            var users = await _directoryService.SearchUsersAsync(filter, cancellationToken);

            var userDtos = users.Select(MapToUserDto).ToList();
            var totalCount = userDtos.Count;
            var paged = userDtos.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new UserSearchResultDto(paged, totalCount));
        }

        [HttpGet("{id}/groups")]
        [ProducesResponseType(typeof(IReadOnlyCollection<GroupDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyCollection<GroupDto>>> GetUserGroups(string id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Group membership lookup for user {Id}", id);

            var groups = await _directoryService.GetUserGroupsAsync(id, cancellationToken);

            var groupDtos = groups.Select(g => new GroupDto(
                g.Id,
                g.Name,
                g.DistinguishedName,
                g.Members
            )).ToList();

            return Ok(groupDtos);
        }

        private static UserDto MapToUserDto(Core.Entities.DirectoryUser user) =>
            new UserDto(
                user.Id,
                user.Username,
                user.DisplayName,
                user.Email,
                user.DistinguishedName,
                user.Enabled,
                user.CreatedAt,
                user.ModifiedAt,
                null  // Attributes omitted to prevent sensitive AD data exposure
            );
    }
}
