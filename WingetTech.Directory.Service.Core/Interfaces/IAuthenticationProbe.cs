namespace WingetTech.Directory.Service.Core.Interfaces;

/// <summary>
/// Provides authentication probing capabilities for directory services.
/// </summary>
public interface IAuthenticationProbe
{
    Task<bool> TestBindAsync(CancellationToken cancellationToken = default);
}
