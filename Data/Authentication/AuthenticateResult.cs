using System;

namespace AHpx.ModuleLauncher.Data.Authentication
{
    public class AuthenticateResult
    {
        public string Name { get; set; }
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
                Name = name,
                Uuid = Guid.NewGuid().ToString("N"),
                AccessToken = Guid.NewGuid().ToString("N"),
                ClientToken = Guid.NewGuid().ToString("N"),
                Verified = true
            };
        }
    }
}