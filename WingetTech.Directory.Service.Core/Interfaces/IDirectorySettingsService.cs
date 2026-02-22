using WingetTech.Directory.Service.Contracts;

namespace WingetTech.Directory.Service.Core.Interfaces
{
    public interface IDirectorySettingsService
    {
        Task SaveAsync(DirectorySettingsDto dto);
        Task<DirectorySettingsDto?> GetAsync();
    }
}
