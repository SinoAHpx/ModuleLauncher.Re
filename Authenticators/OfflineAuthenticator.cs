using AHpx.ModuleLauncher.Data.Authentication;

namespace AHpx.ModuleLauncher.Authenticators
{
    public class OfflineAuthenticator
    {
        public string Name { get; set; }

        public AuthenticateResult Authenticate()
        {
            return Name;
        }
    }
}