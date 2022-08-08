namespace ModuleLauncher.NET.Authentications;

public sealed class OfflineAccount : MinecraftAccount
{
    public new string Name { get; init; }

    public override async Task AuthenticateAsync(string? code = null,
        RefreshableAuthenticateResult.AuthenticationRefreshRequired? authenticationRefreshRequired = null)
    {
        AuthenticationCredential = new RefreshableAuthenticateResult(new OfflineAuthenticator(Name).Authenticate());
    }

    public OfflineAccount(string name)
    {
        Name = name;
        AuthenticateAsync().Wait();
    }
}