namespace WingetTech.Directory.Service.Contracts
{
    public record GroupDto(
        string Id,
        string Name,
        string? DistinguishedName,
        IReadOnlyCollection<string> Members,
        string? Description = null
    );
}
