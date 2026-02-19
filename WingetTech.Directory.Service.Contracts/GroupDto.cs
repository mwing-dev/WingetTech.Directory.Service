namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Data transfer object representing a directory group.
/// </summary>
/// <param name="Id">The unique identifier for the group.</param>
/// <param name="Name">The name of the group.</param>
/// <param name="DistinguishedName">The distinguished name of the group in the directory.</param>
/// <param name="Members">Collection of user IDs that are members of this group.</param>
public record GroupDto(
    string Id,
    string Name,
    string? DistinguishedName,
    IReadOnlyCollection<string> Members
);
