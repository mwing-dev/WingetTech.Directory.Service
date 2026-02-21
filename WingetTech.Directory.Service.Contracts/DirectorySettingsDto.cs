using System.ComponentModel.DataAnnotations;

namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Data transfer object for directory connection settings.
/// </summary>
public class DirectorySettingsDto
{
    [Required]
    public string Host { get; set; } = default!;

    [Range(1, 65535)]
    public int Port { get; set; }

    public bool UseSsl { get; set; }

    [Required]
    public string Domain { get; set; } = default!;

    [Required]
    public string BaseDn { get; set; } = default!;

    [Required]
    public string BindUsername { get; set; } = default!;

    [Required]
    public string BindPassword { get; set; } = default!;
}
