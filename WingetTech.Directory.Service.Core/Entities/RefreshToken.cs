namespace WingetTech.Directory.Service.Core.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = default!;
    public string Username { get; set; } = default!;
    public DateTime ExpiresUtc { get; set; }
    public DateTime CreatedUtc { get; set; }
    public bool IsRevoked { get; set; }
}
