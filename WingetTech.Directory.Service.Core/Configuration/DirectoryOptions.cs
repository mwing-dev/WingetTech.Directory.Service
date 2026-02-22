namespace WingetTech.Directory.Service.Core.Configuration
{
    public class DirectoryOptions
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 389;
        public bool UseSsl { get; set; }
        public string BaseDn { get; set; } = string.Empty;
        public string? BindDn { get; set; }
        public string? BindPassword { get; set; }
    }
}
