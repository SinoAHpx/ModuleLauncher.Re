using Flurl.Http;
using Manganese.Text;
using ModuleLauncher.NET.Models.Authentication;
using ModuleLauncher.NET.Models.Exceptions;
using Newtonsoft.Json;

namespace ModuleLauncher.NET.Authentications;

/// <summary>
/// An implementation of microsoft OAuth for Minecraft
/// </summary>
public class MicrosoftAuthenticator
{
    #region Exposed properties

    /// <summary>
    /// If you don't specify your azure id, Mojang's will be used as default
    /// </summary>
    public string ClientId { get; set; } = "00000000402b5328";

    /// <summary>
    /// If you don't specify your redirect url, a default one will be used
    /// </summary>
    public string RedirectUrl { get; set; } = "https://login.live.com/oauth20_desktop.srf";

    /// <summary>
    /// The code from the browser
    /// <example>M.R3_BAY.415395f4-181b-8f6e-3ec7-b4749c13c742</example>
    /// <remarks>Check the documentation if you don't really know what is this</remarks>
    /// </summary>
    public string Code { get; set; }

    #endregion

    #region Consts

    /// <summary>
    /// Microsoft login url, this procedure needs to be done in browser/webview
    /// </summary>
    public string LoginUrl =>
        $"https://login.live.com/oauth20_authorize.srf?client_id={ClientId}&response_type=code&redirect_uri=https://login.live.com/oauth20_desktop.srf&scope=XboxLive.signin%20offline_access";

    #endregion

    /// <summary>
    /// Exchange code for authorization token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FailedAuthenticationException"></exception>
    private async Task<(string AccessToken, string RefreshToken)> GetAuthorizationTokenAsync(string? token = null)
    {
        var endpoint = "https://login.live.com/oauth20_token.srf";
        object payload = string.IsNullOrWhiteSpace(token)
            ? new
            {
                client_id = ClientId,
                code = Code,
                grant_type = "authorization_code",
                redirect_uri = RedirectUrl,
            }
            : new
            {
                client_id = ClientId,
                refresh_token = token,
                grant_type = "refresh_token",
                redirect_uri = RedirectUrl
            };
        
        var response = await endpoint.PostUrlEncodedAsync(payload);

        try
        {
            var responseJson = await response.GetStringAsync();
            var accessToken = responseJson.Fetch("access_token");
            var refreshToken = responseJson.Fetch("refresh_token");

            if (string.IsNullOrWhiteSpace(responseJson) || string.IsNullOrWhiteSpace(accessToken) ||
                string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new FailedAuthenticationException("Response JSON is invalid");
            }

            return (accessToken, refreshToken);
        }
        catch (Exception e)
        {
            throw new FailedAuthenticationException("Failed to authorize", e);
        }
    }

    /// <summary>
    /// Get Xbox live token & userhash
    /// </summary>
    /// <returns></returns>
    private async Task<(string XNLToken, string XBLUhs)> AuthenticateXBLAsync(string token)
    {
        try
        {
            // var token = (await GetAuthorizationTokenAsync()).access_token;
            var endpoint = "https://user.auth.xboxlive.com/user/authenticate";
            var rpsTicket = $"d={token}";

            var payload = new
            {
                Properties = new
                {
                    AuthMethod = "RPS",
                    SiteName = "user.auth.xboxlive.com",
                    RpsTicket = rpsTicket
                },
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT"
            };

            var response = await endpoint.PostJsonAsync(payload);
            var json = await response.GetStringAsync();
            var xblToken = json.Fetch("Token");
            var xblUhs = json.FetchJToken("DisplayClaims.xui")?.First?.Fetch("uhs");

            if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(xblToken) ||
                string.IsNullOrWhiteSpace(xblUhs))
            {
                throw new FailedAuthenticationException("Invalid JSON response");
            }

            return (xblToken, xblUhs);
        }
        catch (Exception e)
        {
            throw new FailedAuthenticationException("Failed to process Xbox live authentication", e);
        }
    }

    /// <summary>
    /// Get Xbox security token service token & userhash
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FailedAuthenticationException"></exception>
    private async Task<(string XSTSToken, string XSTSUhs)> AuthenticateXSTSAsync(string token)
    {
        try
        {
            var xblAuth = await AuthenticateXBLAsync(token);
            var endpoint = "https://xsts.auth.xboxlive.com/xsts/authorize";
            var payload = new
            {
                Properties = new
                {
                    SandboxId = "RETAIL",
                    UserTokens = new[]
                    {
                        xblAuth.XNLToken
                    }
                },
                RelyingParty = "rp://api.minecraftservices.com/",
                TokenType = "JWT"
            };

            var response = await endpoint.PostJsonAsync(payload);
            var json = await response.GetStringAsync();
            var xstsToken = json.Fetch("Token");
            var xstsUhs = json.FetchJToken("DisplayClaims.xui")?.First?.Fetch("uhs");

            if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(xstsToken) ||
                string.IsNullOrWhiteSpace(xstsUhs))
            {
                if (json.IsValidJson())
                {
                    HandleXSTSErrorCode(json);
                }

                throw new FailedAuthenticationException("Invalid JSON response");
            }

            return (xstsToken, xstsUhs);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    private async Task<(string AccessToken, TimeSpan ExpiresIn)> AuthenticateMinecraftAsync(string token)
    {
        try
        {
            var xstsAuth = await AuthenticateXSTSAsync(token);
            var endpoint = "https://api.minecraftservices.com/authentication/login_with_xbox";
            var payload = new
            {
                identityToken = $"XBL3.0 x={xstsAuth.XSTSUhs};{xstsAuth.XSTSToken}"
            };

            var response = await endpoint.PostJsonAsync(payload);
            var json = await response.GetStringAsync();

            var accessToken = json.Fetch("access_token");
            var expireTime = json.Fetch("expires_in");

            if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(accessToken) ||
                string.IsNullOrWhiteSpace(expireTime))
            {
                throw new FailedAuthenticationException("Invalid response JSON");
            }

            return (accessToken, TimeSpan.FromSeconds(double.Parse(expireTime)));
        }
        catch (Exception e)
        {
            throw new FailedAuthenticationException("Failed to authenticate with Minecraft", e);
        }
    }

    /// <summary>
    /// Get player's minecraft profile
    /// </summary>
    /// <param name="accessToken">Minecraft access token</param>
    /// <returns></returns>
    /// <exception cref="FailedAuthenticationException">If authenticated user don't have minecraft, the exception will be thrown</exception>
    public async Task<MinecraftProfile> GetMinecraftProfileAsync(string accessToken)
    {
        var endpoint = "https://api.minecraftservices.com/minecraft/profile";
        var response = await endpoint
            .WithHeader("Authorization", $"Bearer {accessToken}")
            .GetStringAsync();

        var re = JsonConvert.DeserializeObject<MinecraftProfile>(response);
        if (re == null)
        {
            throw new FailedAuthenticationException("Failed to retrieve Minecraft profile");
        }

        return re;
    }

    /// <summary>
    /// Check if authenticated user have minecraft
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public async Task<bool> CheckOwnershipAsync(string accessToken)
    {
        try
        {
            _ = await GetMinecraftProfileAsync(accessToken);
            return true;
        }
        catch (FailedAuthenticationException)
        {
            return false;
        }
    }

    /// <summary>
    /// Authenticate minecraft
    /// </summary>
    /// <returns></returns>
    public async Task<AuthenticateResult> AuthenticateAsync()
    {
        var token = await GetAuthorizationTokenAsync();
        var mcAuth = await AuthenticateMinecraftAsync(token.AccessToken);
        var profile = await GetMinecraftProfileAsync(mcAuth.AccessToken);

        return new AuthenticateResult
        {
            Name = profile.Name,
            AccessToken = mcAuth.AccessToken,
            UUID = profile.Id,
            RefreshToken = token.RefreshToken,
            ExpireIn = mcAuth.ExpiresIn
        };
    }

    /// <summary>
    /// Refresh authentication, getting a new access token after old one expired
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public async Task<AuthenticateResult> RefreshAuthenticateAsync(string refreshToken)
    {
        var token = await GetAuthorizationTokenAsync(refreshToken);
        var mcAuth = await AuthenticateMinecraftAsync(token.AccessToken);
        var profile = await GetMinecraftProfileAsync(mcAuth.AccessToken);

        return new AuthenticateResult
        {
            Name = profile.Name,
            AccessToken = mcAuth.AccessToken,
            UUID = profile.Id,
            RefreshToken = token.RefreshToken,
            ExpireIn = mcAuth.ExpiresIn
        };
    }

    private void HandleXSTSErrorCode(string json)
    {
        var errorCode = json.Fetch("XErr");
        if (string.IsNullOrWhiteSpace(errorCode))
        {
            throw new FailedAuthenticationException("Invalid JSON response");
        }

        switch (errorCode)
        {
            case "2148916233":
                throw new FailedAuthenticationException(
                    "The account doesn't have an Xbox account. Once they sign up for one (or login through minecraft.net to create one) then they can proceed with the login. This shouldn't happen with accounts that have purchased Minecraft with a Microsoft account, as they would've already gone through that Xbox signup process.");
            case "2148916235":
                throw new FailedAuthenticationException(
                    "The account is from a country where Xbox Live is not available/banned");
            case "2148916236":
            case "2148916237":
                throw new FailedAuthenticationException("The account needs adult verification on Xbox page.");
            case "2148916238":
                throw new FailedAuthenticationException(
                    "The account is a child (under 18) and cannot proceed unless the account is added to a Family by an adult. This only seems to occur when using a custom Microsoft Azure application. When using the Minecraft launchers client id, this doesn't trigger.");
            default:
                throw new FailedAuthenticationException(
                    $"Authenticate failed with code {errorCode}: {json.Fetch("Message")}");
        }
    }
}