namespace WingetTech.Directory.Service.Core.Entities;

/// <summary>
/// Represents a group in the directory service.
/// </summary>
public class DirectoryGroup
{
    /// <summary>
    /// Gets or sets the unique identifier for the group.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the distinguished name of the group in the directory.
    /// </summary>
    public string? DistinguishedName { get; set; }

    /// <summary>
    /// Gets or sets the description of the group.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the collection of user IDs that are members of this group.
    /// </summary>
    public IReadOnlyCollection<string> Members { get; set; } = Array.Empty<string>();
}
