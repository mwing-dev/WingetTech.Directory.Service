namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Data transfer object representing an organizational unit in the directory.
/// </summary>
/// <param name="Name">The name of the organizational unit.</param>
/// <param name="DistinguishedName">The distinguished name of the organizational unit.</param>
/// <param name="ParentDn">The distinguished name of the parent organizational unit.</param>
public record OrganizationalUnitDto(
    string Name,
    string? DistinguishedName,
    string? ParentDn
);
