namespace WingetTech.Directory.Service.Core.Entities;

public class DirectoryUser
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? DistinguishedName { get; set; }
    public bool Enabled { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public IReadOnlyDictionary<string, string>? Attributes { get; set; }
}
