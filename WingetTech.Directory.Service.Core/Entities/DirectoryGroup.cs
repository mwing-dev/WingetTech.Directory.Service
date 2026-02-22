namespace WingetTech.Directory.Service.Core.Entities
{
    public class DirectoryGroup
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? DistinguishedName { get; set; }
        public string? Description { get; set; }
        public IReadOnlyCollection<string> Members { get; set; } = Array.Empty<string>();
    }
}
