using WingetTech.Directory.Service.Contracts;

namespace WingetTech.Directory.Service.Core.Interfaces
{
    public interface IDirectorySettingsService
    {
        Task SaveAsync(DirectorySettingsDto dto, CancellationToken cancellationToken = default);
        Task<DirectorySettingsDto?> GetAsync(CancellationToken cancellationToken = default);
        Task<bool> HasSettingsAsync(CancellationToken cancellationToken = default);
    }
}
