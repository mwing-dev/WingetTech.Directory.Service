namespace WingetTech.Directory.Service.Core.Interfaces;

/// <summary>
/// Provides authentication probing capabilities for directory services.
/// </summary>
public interface IAuthenticationProbe
{
    /// <summary>
    /// Tests authentication by attempting to bind with the provided credentials.
    /// </summary>
    /// <param name="username">The username to test.</param>
    /// <param name="password">The password to test.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the bind was successful.</returns>
    Task<bool> TestBindAsync(string username, string password, CancellationToken cancellationToken = default);
}
