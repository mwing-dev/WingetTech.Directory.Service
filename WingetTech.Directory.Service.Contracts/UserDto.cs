namespace WingetTech.Directory.Service.Contracts;

public record UserDto(
    string Id,
    string Username,
    string? DisplayName,
    string? Email,
    string? DistinguishedName,
    bool Enabled,
    DateTime? CreatedAt,
    DateTime? ModifiedAt,
    IReadOnlyDictionary<string, string>? Attributes
);
