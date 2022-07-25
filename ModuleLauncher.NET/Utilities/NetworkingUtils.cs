namespace ModuleLauncher.NET.Utilities;

public static class NetworkingUtils
{
    private static readonly HttpClient HttpClient = new();

    /// <summary>
    /// Send a http-get request to the specified url and retrieve the string
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async Task<string> GetStringAsync(this Uri url)
    {
        return await HttpClient.GetStringAsync(url);
    }
    
    /// <summary>
    /// Send a http-get request to the specified url and retrieve the string
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async Task<string> GetStringAsync(this string url)
    {
        return await HttpClient.GetStringAsync(url);
    }

    /// <summary>
    /// Send a http-get request and retrieve the HttpResponseMessage
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> GetAsync(this Uri uri)
    {
        var responseMessage = await HttpClient.GetAsync(uri);
        responseMessage.EnsureSuccessStatusCode();
        
        return responseMessage;
    }
    
    /// <summary>
    /// Send a http-get request and retrieve the HttpResponseMessage
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> GetAsync(this string uri)
    {
        var responseMessage = await HttpClient.GetAsync(uri);
        responseMessage.EnsureSuccessStatusCode();
        
        return responseMessage;
    }
}