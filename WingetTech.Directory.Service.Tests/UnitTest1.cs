using WingetTech.Directory.Service.Core;
using WingetTech.Directory.Service.Infrastructure;

namespace WingetTech.Directory.Service.Tests;

public class LdapDirectoryServiceTests
{
    public void LdapDirectoryService_Constructor_InitializesSuccessfully()
    {
    }

    public async Task AuthenticateAsync_ThrowsNotImplementedException()
    {
        await Task.CompletedTask;
    }
}

