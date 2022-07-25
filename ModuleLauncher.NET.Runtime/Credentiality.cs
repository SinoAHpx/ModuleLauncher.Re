using Newtonsoft.Json;

namespace ModuleLauncher.NET.Runtime;

public static class Credentiality
{
    public static string? Path;

    private static void InitializedPath<T>()
    {
        Path ??= Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $"\\{typeof(T).Name}.json";
    }

    public static void CreateCredential<T>(this T t)
    {
        InitializedPath<T>();

        var json = JsonConvert.SerializeObject(t);
        File.WriteAllText(Path, json);
    }

    public static T ReadCredential<T>()
    {
        InitializedPath<T>();

        var text = File.ReadAllText(Path);
        return JsonConvert.DeserializeObject<T>(text)!;
    }
}

public class OAuthInfo
{
    public string ClientId { get; set; }
}