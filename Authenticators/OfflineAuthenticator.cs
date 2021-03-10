using AHpx.ModuleLauncher.Data.Authentications;

namespace AHpx.ModuleLauncher.Authenticators
{
    public class OfflineAuthenticator
    {
        public OfflineAuthenticator(string name = null)
        {
            Name = name;
        }

        public string Name { get; set; }

        public AuthenticateResult Authenticate()
        {
            return Name;
        }
    }
}