using System.Text;
using Flurl.Http;
using Manganese.Data;
using Manganese.Text;
using ModuleLauncher.NET.Models.Authentication;
using Newtonsoft.Json;

namespace ModuleLauncher.NET.Utilities;

public static class MojangApiUtils
{
    /// <summary>
    /// Throw an InvalidOperationException when response json is null
    /// </summary>
    /// <param name="s"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private static string Process(this string? s)
    {
        if (s.IsNullOrEmpty())
        {
            throw new InvalidOperationException("Invalid response json");
        }

        if (!s.IsJArray() && !s.Fetch("errorMessage").IsNullOrEmpty())
        {
            throw new InvalidOperationException(s.Fetch("errorMessage"));
        }

        return s;
    }
    
    /// <summary>
    /// Return uuid if player exists
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public static async Task<string> GetUuidByUsernameAsync(string username)
    {
        var endpoint = $"https://api.mojang.com/users/profiles/minecraft/{username}";
        var response = await endpoint.GetStringAsync();
        var uuid = response.Process().Fetch("id");

        return uuid.ThrowIfNullOrEmpty<InvalidOperationException>();
    }

    /// <summary>
    /// This will return player UUIDs and some extras
    /// <remarks>BadRequestException is returned when any of the usernames is null or otherwise invalid</remarks>
    /// </summary>
    /// <param name="usernames">You cannot request more than 10 names per request</param>
    /// <returns></returns>
    public static async Task<List<string>> GetUuidsByUsernamesAsync(params string[] usernames)
    {
        var endpoint = "https://api.mojang.com/profiles/minecraft";
        var response = await endpoint.PostJsonAsync(usernames).ReceiveString();

        var array = response.Process().ToJArray();
        var uuids = array.Select(t => t.Fetch("id").ThrowIfNullOrEmpty<InvalidCastException>()).ToList();
        return uuids;
    }

    /// <summary>
    /// This will return player UUIDs and some extras
    /// <remarks>BadRequestException is returned when any of the usernames is null or otherwise invalid</remarks>
    /// </summary>
    /// <param name="usernames">You cannot request more than 10 names per request</param>
    /// <returns></returns>
    public static async Task<List<string>> GetUuidsByUsernamesAsync(IEnumerable<string> usernames)
    {
        return await GetUuidsByUsernamesAsync(usernames.ToArray());
    }

    /// <summary>
    /// Get player's name history by uuid
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public static async Task<List<(string name, DateTime? time)>> GetNameHistoryByUuidAsync(string uuid)
    {
        var endpoint = $"https://api.mojang.com/user/profiles/{uuid}/names";
        var response = await endpoint.GetStringAsync();
        var reponseArray = response.Process().ToJArray();

        var re = reponseArray.Select(
            t => (t.Fetch("name").ThrowIfNullOrEmpty<InvalidOperationException>(),
                CommonUtils.UnixTimeStampToDateTime(t.Fetch("changedToAt"))));

        return re.ToList();
    }

    /// <summary>
    /// This will return the player's username plus any additional information about them (e.g. skins)
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public static async Task<MinecraftProfile> GetProfileByUuidAsync(string uuid)
    {
        var endpoint = $"https://sessionserver.mojang.com/session/minecraft/profile/{uuid}";
        var rawJson = (await endpoint.GetStringAsync()).ThrowIfNullOrEmpty<InvalidOperationException>();

        var base64 = rawJson.FetchJToken("properties")?.First?.Fetch("value")
            .ThrowIfNullOrEmpty<InvalidOperationException>();

        var decodedJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64!));
        var profile = new MinecraftProfile
        {
            Id = uuid,
            Name = decodedJson.Fetch("name")!
        };

        var skin = decodedJson.Fetch("textures.SKIN.url");
        if (!skin.IsNullOrEmpty())
            profile.skins = new List<MinecraftProfile.AccountTesture> { new() { Url = skin } };

        var cape = decodedJson.Fetch("textures.CAPE.url");
        if (!cape.IsNullOrEmpty())
            profile.Capes = new List<MinecraftProfile.AccountTesture> { new() { Url = cape } };

        return profile;
    }

    /// <summary>
    /// This fetches information about the profile name such as the date the name was changed and the date the account was created
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public static async Task<(DateTime ChangedAt, DateTime CreatedAt, bool NameChangeAllowed)> GetProfileNameChangeInfoAsync(string accessToken)
    {
        var endpoint = "https://api.minecraftservices.com/minecraft/profile/namechange";
        var response = (await endpoint.WithOAuthBearerToken(accessToken).GetStringAsync()).Process();

        return (DateTime.Parse(response.Fetch("changedAt")!), DateTime.Parse(response.Fetch("createdAt")!),
            bool.Parse(response.Fetch("nameChangeAllowed")!));
    }
    
    /// <summary>
    /// Change minecraft username
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="changeTo"></param>
    /// <returns></returns>
    public static async Task<MinecraftProfile> ChangeUsernameAsync(string accessToken, string changeTo)
    {
        var endpoint = $"https://api.minecraftservices.com/minecraft/profile/name/{changeTo}";
        var response = await endpoint.WithOAuthBearerToken(accessToken).PutAsync().ReceiveString();

        return JsonConvert.DeserializeObject<MinecraftProfile>(response.Process())
            .ThrowIfNull(new InvalidOperationException("Invalid response json"));
    }

    /// <summary>
    /// This API endpoint checks if the given name is available to change
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static async Task<bool> CheckNameAvailabilityAsync(string accessToken, string name)
    {
        var endpoint = $"https://api.minecraftservices.com/minecraft/profile/name/{name}/available";
        var response = (await endpoint.WithOAuthBearerToken(accessToken).GetStringAsync())
            .ThrowIfNullOrEmpty<InvalidOperationException>()
            .Process();

        return response.Fetch("status")?.ToLower() is "available";
    }
}