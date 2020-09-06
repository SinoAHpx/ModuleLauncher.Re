using ModuleLauncher.Re.Utils.DataEntity.Authenticator;

namespace ModuleLauncher.Re.Authenticator
{
    //head
    public partial class OfflineAuthenticator
    {
        public string Name { get; set; }

        public OfflineAuthenticator(string name = "")
        {
            Name = name;
        }
    }

    public partial class OfflineAuthenticator
    {
        public AuthenticateResult Authenticate()
        {
            return Name;
        }
    }
}