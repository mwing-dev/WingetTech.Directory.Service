namespace WingetTech.Directory.Service.Core.Entities;

public class DirectoryOrganizationalUnit
{
    public string Name { get; set; } = string.Empty;
    public string? DistinguishedName { get; set; }
    public string? ParentDn { get; set; }
}
