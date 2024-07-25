using Flurl.Http;
using Manganese.Text;
using ModuleLauncher.NET.Models.Authentication;
using ModuleLauncher.NET.Models.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ModuleLauncher.NET.Authentications;

/// <summary>
/// An implementation of microsoft OAuth for Minecraft
/// </summary>
public class MicrosoftAuthenticator
{

    /// <summary>
    /// Azure application cliend id
    /// </summary>
    public string ClientId { get; set; }

    public async Task<DeviceCodeInfo?> GetDeviceCodeAsync()
    {
        var url = "https://login.microsoftonline.com/consumers/oauth2/v2.0/devicecode";
        var result = await url
            .PostUrlEncodedAsync(new
            {
                client_id = ClientId,
                scope = "XboxLive.signin"
            });

        var response = await result.GetStringAsync();
        if (response.IsNullOrEmpty())
        {
            return null;
        }

        var userCode = response.Fetch("user_code");
        if (userCode.IsNullOrEmpty())
        {
            return null;
        }
        var deviceCode = response.Fetch("device_code");
        if (deviceCode.IsNullOrEmpty())
        {
            return null;
        }
        var userUrl = response.Fetch("verification_uri");
        if (userUrl.IsNullOrEmpty())
        {
            return null;
        }

        return new DeviceCodeInfo
        {
            DeviceCode = deviceCode,
            UserCode = userCode,
            VerificationUrl = userUrl
        };
    }

    public async Task<string?> PollAuthorizationAsync(DeviceCodeInfo deviceCodeInfo)
    {
        var pollingUrl = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";
        string? accessToken = null;
        await Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    var pollingResult = await pollingUrl.PostUrlEncodedAsync(new
                    {
                        grant_type = "urn:ietf:params:oauth:grant-type:device_code",
                        client_id = ClientId,
                        device_code = deviceCodeInfo.DeviceCode
                    });
                    var body = await pollingResult.GetStringAsync();
                    if (body.Fetch("access_token") is {} token)
                    {
                        accessToken = token;
                        break;
                    }
                    await Task.Delay(1000);
                }
                catch { /* do nothing */ }
            }
        });

        return accessToken;
    }

    public async Task<AuthenticateResult?> AuthenticateAsync(string oauthAccessToken)
    {
        try
        {
            var xblToken = await GetXBLTokenAsync(oauthAccessToken);
            var xsts = await GetXSTSTokenAsync(xblToken);
            var minecraftAccessToken = await GetMinecraftAccessTokenAsync(xsts);
            var profile = await GetMinecraftProfileAsync(minecraftAccessToken.AccessToken);

            return new AuthenticateResult
            {
                AccessToken = minecraftAccessToken.AccessToken,
                Name = profile.Name,
                ExpireIn = minecraftAccessToken.ExpiresIn,
                UUID = profile.Id
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get Xbox live token & userhash
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetXBLTokenAsync(string token)
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
            // var xblUhs = json.FetchJToken("DisplayClaims.xui")?.First?.Fetch("uhs");

            if (json.IsNullOrEmpty() || xblToken.IsNullOrEmpty())
            {
                throw new FailedAuthenticationException("Failed to authenticate with XBox live, check out your Minecraft authentication.");
            }

            return xblToken;
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
    private async Task<(string XSTSToken, string XSTSUhs)> GetXSTSTokenAsync(string xblToken)
    {
        try
        {
            var endpoint = "https://xsts.auth.xboxlive.com/xsts/authorize";
            var payload = new
            {
                Properties = new
                {
                    SandboxId = "RETAIL",
                    UserTokens = new[]
                    {
                        xblToken
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

    private async Task<(string AccessToken, TimeSpan ExpiresIn)> GetMinecraftAccessTokenAsync((string token, string userhash) xsts)
    {
        try
        {
            var endpoint = "https://api.minecraftservices.com/authentication/login_with_xbox";
            var payload = new
            {
                identityToken = $"XBL3.0 x={xsts.userhash};{xsts.token}"
            };

            var response = await endpoint.PostJsonAsync(payload);
            var json = await response.GetStringAsync();

            var accessToken = json.Fetch("access_token");
            var expireTime = json.Fetch("expires_in");

            if (json.IsNullOrEmpty() || accessToken.IsNullOrEmpty() ||
                expireTime.IsNullOrEmpty())
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


    // /// <summary>
    // /// Refresh authentication, getting a new access token after old one expired
    // /// </summary>
    // /// <param name="refreshToken"></param>
    // /// <returns></returns>
    // public async Task<AuthenticateResult> RefreshAuthenticateAsync(string refreshToken)
    // {
    //     var token = await GetAuthorizationTokenAsync(refreshToken);
    //     var mcAuth = await AuthenticateMinecraftAsync(token.AccessToken);
    //     var profile = await GetMinecraftProfileAsync(mcAuth.AccessToken);
    //
    //     return new AuthenticateResult
    //     {
    //         Name = profile.Name,
    //         AccessToken = mcAuth.AccessToken,
    //         UUID = profile.Id,
    //         RefreshToken = token.RefreshToken,
    //         ExpireIn = mcAuth.ExpiresIn
    //     };
    // }

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