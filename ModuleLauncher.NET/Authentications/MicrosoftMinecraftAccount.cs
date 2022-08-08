using ModuleLauncher.NET.Models.Authentication;

namespace ModuleLauncher.NET.Authentications;

public sealed class MicrosoftMinecraftAccount : MinecraftAccount
{
    public new string Name => AuthenticationCredential.Name;

    public MicrosoftAuthenticator Authenticator { get; }

    public bool IsAvailable => !string.IsNullOrEmpty(AuthenticationCredential.AccessToken) &&
                               !string.IsNullOrEmpty(AuthenticationCredential.UUID);

    public override async Task AuthenticateAsync(string? code = null,
        RefreshableAuthenticateResult.AuthenticationRefreshRequired? authenticationRefreshRequired = null)
    {
        AuthenticationCredential = new RefreshableAuthenticateResult(await Authenticator.AuthenticateAsync());
        if (authenticationRefreshRequired is not null)
            AuthenticationCredential.OnRefreshRequired += authenticationRefreshRequired;
        else
            AuthenticationCredential.OnRefreshRequired += async (authentication) =>
                await Authenticator.RefreshAuthenticateAsync(authentication.RefreshToken);
    }

    public MicrosoftMinecraftAccount(string code,
        RefreshableAuthenticateResult.AuthenticationRefreshRequired? authenticationRefreshRequired = null)
    {
        Authenticator = new MicrosoftAuthenticator { Code = code };
        AuthenticateAsync(code, authenticationRefreshRequired).Wait();
    }
}