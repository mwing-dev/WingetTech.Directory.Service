namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Data transfer object representing search results for groups.
/// </summary>
/// <param name="Groups">The collection of groups matching the search criteria.</param>
/// <param name="TotalCount">The total number of groups found.</param>
public record GroupSearchResultDto(
    IReadOnlyCollection<GroupDto> Groups,
    int TotalCount
);
