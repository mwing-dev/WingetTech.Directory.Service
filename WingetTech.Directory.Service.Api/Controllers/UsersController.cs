using Microsoft.AspNetCore.Mvc;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Api.Controllers;

/// <summary>
/// Controller for managing directory users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IDirectoryService _directoryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    /// <param name="directoryService">The directory service.</param>
    public UsersController(IDirectoryService directoryService)
    {
        _directoryService = directoryService;
    }

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The user if found; otherwise, a 404 Not Found response.</returns>
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

    /// <summary>
    /// Gets a user by their username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The user if found; otherwise, a 404 Not Found response.</returns>
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

    /// <summary>
    /// Searches for users matching the specified filter.
    /// </summary>
    /// <param name="filter">The search filter to apply.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A collection of users matching the search criteria.</returns>
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

    /// <summary>
    /// Gets all groups that a user is a member of.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A collection of groups the user belongs to.</returns>
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
