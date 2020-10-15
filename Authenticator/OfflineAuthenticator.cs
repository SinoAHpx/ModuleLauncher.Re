using ModuleLauncher.Re.DataEntities.Authenticator;

namespace ModuleLauncher.Re.Authenticator
{
    //head
    public partial class OfflineAuthenticator
    {
        public OfflineAuthenticator(string name = "")
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public partial class OfflineAuthenticator
    {
        public AuthenticateResult Authenticate()
        {
            return Name;
        }
    }
}