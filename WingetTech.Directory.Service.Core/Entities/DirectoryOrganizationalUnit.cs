namespace WingetTech.Directory.Service.Core.Entities;

/// <summary>
/// Represents an organizational unit in the directory service.
/// </summary>
public class DirectoryOrganizationalUnit
{
    /// <summary>
    /// Gets or sets the name of the organizational unit.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the distinguished name of the organizational unit.
    /// </summary>
    public string? DistinguishedName { get; set; }

    /// <summary>
    /// Gets or sets the distinguished name of the parent organizational unit.
    /// </summary>
    public string? ParentDn { get; set; }
}
