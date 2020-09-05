using Newtonsoft.Json;

namespace ModuleLauncher.Re.Service
{
    internal class AuthenticatorPayload
    {
        internal string Username { get; set; }
        internal string Password { get; set; }
        internal string ClientToken { get; set; }

        internal string GetAuthenticatePayload()
        {
            return JsonConvert.SerializeObject(new
            {
                agent = new
                {
                    name = "Minecraft",
                    version = 1
                },
                username = Username,
                password = Password,
                clientToken = ClientToken
            });
        }

        internal string GetRefreshPayload(string accessToken)
        {
            return JsonConvert.SerializeObject(new
            {
                accessToken = accessToken,
                clientToken = ClientToken
            });
        }
    }
}