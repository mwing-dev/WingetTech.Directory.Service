namespace WingetTech.Directory.Service.Core.Interfaces;

/// <summary>
/// Provides authentication probing capabilities for directory services.
/// </summary>
public interface IAuthenticationProbe
{
    /// <summary>
    /// Tests the LDAP bind using the persisted <c>DirectorySettings</c>.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>
    /// A tuple of (<c>success</c>, <c>errorMessage</c>) where <c>errorMessage</c> is
    /// <c>null</c> on success and contains a diagnostic message on failure.
    /// </returns>
    Task<(bool Success, string? ErrorMessage)> TestBindAsync(CancellationToken cancellationToken = default);
}
