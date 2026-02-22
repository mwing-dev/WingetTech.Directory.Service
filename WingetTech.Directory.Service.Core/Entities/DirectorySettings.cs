namespace WingetTech.Directory.Service.Core.Entities;

public class DirectorySettings
{
    public Guid Id { get; set; }
    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    public string Domain { get; set; } = default!;
    public string BaseDn { get; set; } = default!;
    public string BindUsername { get; set; } = default!;
    public string BindPassword { get; set; } = default!;
    public DateTime UpdatedUtc { get; set; }
}
