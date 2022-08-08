using Manganese.Data;
using ModuleLauncher.NET.Models.Authentication;
using Timer = System.Timers.Timer;

namespace ModuleLauncher.NET.Authentications;

public class RefreshableAuthenticateResult : AuthenticateResult
{
    public delegate Task<AuthenticateResult> AuthenticationRefreshRequired(AuthenticateResult originalAuthentication);

    public event AuthenticationRefreshRequired OnRefreshRequired = (_) => null;

    private System.Timers.Timer? RefreshTimer;

    public RefreshableAuthenticateResult(AuthenticateResult authenticateResult)
    {
        Name = authenticateResult.Name;
        UUID = authenticateResult.UUID;
        AccessToken = authenticateResult.AccessToken;
        RefreshToken = authenticateResult.RefreshToken;
        ExpireIn = authenticateResult.ExpireIn;
        ClientToken = authenticateResult.ClientToken;
        if (ExpireIn == TimeSpan.Zero) return; //实例化Timer类，设置间隔时间为10000毫秒； 
        RefreshTimer = new Timer(ExpireIn.TotalMilliseconds);
        RefreshTimer.Elapsed += async (sender, args) =>
        {
            var result = await OnRefreshRequired(this).ThrowIfNull();
            Name = result.Name;
            UUID = result.UUID;
            AccessToken = result.AccessToken;
            RefreshToken = result.RefreshToken;
            ExpireIn = result.ExpireIn;
            ClientToken = result.ClientToken;
        };
        RefreshTimer.AutoReset = true;
        RefreshTimer.Enabled = true;
    }
}