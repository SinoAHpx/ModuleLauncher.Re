using System;

namespace ModuleLauncher.Re.Service.DataEntity.Authenticator
{
    public class AuthenticateResult
    {
        public string Username { get; set; }
        public string Uuid { get; set; }
        public string AccessToken { get; set; }
        public string ClientToken { get; set; }

        public string Error { get; set; }
        public string ErrorMessage { get; set; }

        public bool Verified { get; set; }

        public static implicit operator AuthenticateResult(string name)
        {
            return new AuthenticateResult
            {
                Username = name,
                Uuid = Guid.NewGuid().ToString("N"),
                AccessToken = Guid.NewGuid().ToString("N"),
                ClientToken = Guid.NewGuid().ToString("N"),
                Verified = true
            };
        }
    }
}