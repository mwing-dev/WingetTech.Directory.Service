namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Data transfer object representing a directory group.
/// </summary>
public class GroupDto
{
    /// <summary>
    /// The unique identifier for the group.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name of the group.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A description of the group's purpose.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The distinguished name of the group in the directory.
    /// </summary>
    public string? DistinguishedName { get; set; }

    /// <summary>
    /// Members of the group.
    /// </summary>
    public List<string> Members { get; set; } = new();
}
