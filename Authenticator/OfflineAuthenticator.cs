using ModuleLauncher.Re.Service.DataEntity.Authenticator;

namespace ModuleLauncher.Re.Authenticator
{
    public class OfflineAuthenticator
    {
        public string Name { get; set; }

        public OfflineAuthenticator(string name = "")
        {
            Name = name;
        }

        public AuthenticateResult Authenticate()
        {
            return Name;
        }
    }
}