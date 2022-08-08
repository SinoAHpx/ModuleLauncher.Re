using System.Security.Cryptography;
using ModuleLauncher.NET.Models.Authentication;

namespace ModuleLauncher.NET.Authentications;

public abstract class MinecraftAccount
{
    public string Name { get; protected set; }

    public RefreshableAuthenticateResult AuthenticationCredential;

    public abstract Task AuthenticateAsync(string? code = null,
        RefreshableAuthenticateResult.AuthenticationRefreshRequired? authenticationRefreshRequired = null);

    public static implicit operator AuthenticateResult(MinecraftAccount account)
    {
        return account.AuthenticationCredential;
    }
}