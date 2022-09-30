using ModuleLauncher.NET.Models.Authentication;

namespace ModuleLauncher.NET.Authentications;

/// <summary>
/// Local authentication without going through network, simply a random UUID and access token generator
/// </summary>
public class OfflineAuthenticator
{
    /// <summary>
    /// Name of the offline player
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Construct with name
    /// </summary>
    /// <param name="name"></param>
    public OfflineAuthenticator(string name = "ml.user")
    {
        Name = name;
    }

    /// <summary>
    /// Generate a random account
    /// <remarks>More convenient way: simply use the implicit type converter</remarks>
    /// </summary>
    /// <returns></returns>
    public AuthenticateResult Authenticate()
    {
        return Name;
    }
}