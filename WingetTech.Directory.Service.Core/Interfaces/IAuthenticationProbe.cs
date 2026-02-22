namespace WingetTech.Directory.Service.Core.Interfaces
{
    public interface IAuthenticationProbe
    {
        Task<bool> TestBindAsync(CancellationToken cancellationToken = default);
        Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
    }
}
