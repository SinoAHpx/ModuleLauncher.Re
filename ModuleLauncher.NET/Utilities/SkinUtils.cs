using Flurl.Http;
using Manganese.Data;
using Manganese.Text;
using ModuleLauncher.NET.Models.Authentication;
using ModuleLauncher.NET.Models.Utils;
using Newtonsoft.Json;

namespace ModuleLauncher.NET.Utilities;

public static class SkinUtils
{
    /// <summary>
    /// Process status code and deserialize to MinecraftProfile
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static async Task<MinecraftProfile> ProcessAsync(this IFlurlResponse response)
    {
        if (response.StatusCode == 400)
        {
            throw new InvalidOperationException((await response.GetStringAsync()).Fetch("errorMessage"));
        }

        var responseJson = await response.GetStringAsync();
        return JsonConvert.DeserializeObject<MinecraftProfile>(responseJson)
            .ThrowIfNull(new InvalidOperationException("Response json is invalid"));
    }
    
    /// <summary>
    /// Change skin selected profile
    /// </summary>
    /// <param name="accessToken">Minecraft access token</param>
    /// <param name="skinUrl">Skin file web address</param>
    /// <param name="variant">Either classic or slim</param>
    public static async Task<MinecraftProfile> ChangeSkinAsync(string accessToken, string skinUrl, SkinVariant variant = SkinVariant.Classic)
    {
        var endpoint = "https://api.minecraftservices.com/minecraft/profile/skins";
        var payload = new
        {
            varient = variant.GetDescription(),
            url = skinUrl
        };

        var response = await endpoint.AllowAnyHttpStatus().WithOAuthBearerToken(accessToken).PostJsonAsync(payload);

        return await response.ProcessAsync();
    }

    /// <summary>
    /// Upload local skin file and set it active
    /// </summary>
    /// <param name="accessToken">Minecraft access token</param>
    /// <param name="skinFile">Must be a local skin file</param>
    /// <param name="skinVariant">Either classic or slim</param>
    public static async Task<MinecraftProfile> ChangeSkinAsync(string accessToken, FileInfo skinFile, SkinVariant skinVariant = SkinVariant.Classic)
    {
        var endpoint = "https://api.minecraftservices.com/minecraft/profile/skins";
        var response = await endpoint.AllowAnyHttpStatus()
            .WithOAuthBearerToken(accessToken).PostMultipartAsync(content =>
                content.AddString("variant", skinVariant.GetDescription())
                    .AddFile("file", skinFile.FullName));

        return await response.ProcessAsync();
    }

    /// <summary>
    /// Reset currently active skin
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public static async Task<MinecraftProfile> ResetSkinAsync(string accessToken)
    {
        var endpoint = "https://api.minecraftservices.com/minecraft/profile/skins/active";
        var response = await endpoint.AllowAnyHttpStatus().WithOAuthBearerToken(accessToken).DeleteAsync();

        return await response.ProcessAsync();
    }

    /// <summary>
    /// Prevents the current cape from being shown on the account
    /// </summary>
    /// <param name="accessToken"></param>
    public static async Task<MinecraftProfile> HideCapeAsync(string accessToken)
    {
        var endpoint = "https://api.minecraftservices.com/minecraft/profile/capes/active";
        var response = await endpoint.AllowAnyHttpStatus().WithOAuthBearerToken(accessToken).DeleteAsync();

        return await response.ProcessAsync();
    }

    /// <summary>
    /// Show cape on current actived skin
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="capeId">Cape id</param>
    /// <returns></returns>
    public static async Task<MinecraftProfile> ShowCapeAsync(string accessToken, string capeId)
    {
        var endpoint = "https://api.minecraftservices.com/minecraft/profile/capes/active";
        var payload = new
        {
            capeId
        };
        var response = await endpoint.AllowAnyHttpStatus().WithOAuthBearerToken(accessToken).PutJsonAsync(payload);

        return await response.ProcessAsync();
    }
}
