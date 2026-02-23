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

        public UsersController(IDirectoryService directoryService)
        {
            _directoryService = directoryService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserById(string id, CancellationToken cancellationToken)
        {
            var user = await _directoryService.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto(
                user.Id,
                user.Username,
                user.DisplayName,
                user.Email,
                user.DistinguishedName,
                user.Enabled,
                user.CreatedAt,
                user.ModifiedAt,
                user.Attributes
            );

            return Ok(userDto);
        }

        [HttpGet("by-username/{username}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserByUsername(string username, CancellationToken cancellationToken)
        {
            var user = await _directoryService.GetUserByUsernameAsync(username, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto(
                user.Id,
                user.Username,
                user.DisplayName,
                user.Email,
                user.DistinguishedName,
                user.Enabled,
                user.CreatedAt,
                user.ModifiedAt,
                user.Attributes
            );

            return Ok(userDto);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(UserSearchResultDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserSearchResultDto>> SearchUsers([FromQuery] string filter, CancellationToken cancellationToken)
        {
            var users = await _directoryService.SearchUsersAsync(filter, cancellationToken);

            var userDtos = users.Select(u => new UserDto(
                u.Id,
                u.Username,
                u.DisplayName,
                u.Email,
                u.DistinguishedName,
                u.Enabled,
                u.CreatedAt,
                u.ModifiedAt,
                u.Attributes
            )).ToList();

            var result = new UserSearchResultDto(userDtos, userDtos.Count);
            return Ok(result);
        }

        [HttpGet("{id}/groups")]
        [ProducesResponseType(typeof(IReadOnlyCollection<GroupDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyCollection<GroupDto>>> GetUserGroups(string id, CancellationToken cancellationToken)
        {
            var groups = await _directoryService.GetUserGroupsAsync(id, cancellationToken);

            var groupDtos = groups.Select(g => new GroupDto(
                g.Id,
                g.Name,
                g.DistinguishedName,
                g.Members
            )).ToList();

            return Ok(groupDtos);
        }
    }
}
