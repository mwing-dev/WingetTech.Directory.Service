namespace WingetTech.Directory.Service.Core.Interfaces
{
    public interface IAuthenticationProbe
    {
        Task<bool> TestBindAsync(CancellationToken cancellationToken = default);
    }
}
