using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;

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
    public string Name { get; set; }

    /// <summary>
    /// User's uuid
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public string UUID { get; set; }

    /// <summary>
    /// User's access token
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// Token for get a new token
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// When this result will be expired
    /// </summary>
    public TimeSpan ExpireIn { get; set; }

    public static implicit operator AuthenticateResult(string incoming)
    {
        return new AuthenticateResult
        {
            Name = incoming,
            AccessToken = Guid.NewGuid().ToString("N"),
            UUID = Guid.NewGuid().ToString("N")
        };
    }
    
    public static implicit operator string(AuthenticateResult result)
    {
        return result.Name.ThrowIfNullOrEmpty<FailedAuthenticationException>("Username cannot be null or empty");
    }
}