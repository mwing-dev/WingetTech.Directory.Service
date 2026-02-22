namespace WingetTech.Directory.Service.Contracts
{
    public record OrganizationalUnitDto(
        string Name,
        string? DistinguishedName,
        string? ParentDn
    );
}
