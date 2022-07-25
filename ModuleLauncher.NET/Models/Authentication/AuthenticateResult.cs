namespace ModuleLauncher.NET.Models.Authentication;

/// <summary>
/// Result of authenticating, contains basic information
/// <remarks>Can be mutually implicitly converted to string</remarks>
/// </summary>
public class AuthenticateResult
{
    /// <summary>
    /// Name of the user
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// User's uuid
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public string? UUID { get; set; }

    /// <summary>
    /// User's access token
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Token for get a new token
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// When this result will be expired
    /// </summary>
    public TimeSpan ExpireIn { get; set; }

    /// <summary>
    /// User's client token, available for legacy authentication or external authenticators
    /// </summary>
    public string? ClientToken { get; set; }

    public static implicit operator AuthenticateResult(string incoming)
    {
        return new AuthenticateResult
        {
            Name = incoming,
            AccessToken = Guid.NewGuid().ToString("N"),
            ClientToken = Guid.NewGuid().ToString("N"),
            UUID = Guid.NewGuid().ToString("N")
        };
    }
    
    public static implicit operator string(AuthenticateResult result)
    {
        return result.Name;
    }
}