namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Data transfer object representing search results for users.
/// </summary>
/// <param name="Users">The collection of users matching the search criteria.</param>
/// <param name="TotalCount">The total number of users found.</param>
public record UserSearchResultDto(
    IReadOnlyCollection<UserDto> Users,
    int TotalCount
);
