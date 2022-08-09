using ModuleLauncher.NET.Models.Authentication;

namespace ModuleLauncher.NET.Authentications;

public interface IAuthenticator
{
    public Task<AuthenticateResult> AuthenticateAsync();
    public Task<AuthenticateResult> RefreshAuthenticateAsync(string token);
}