using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests
{
    public class IdServerConfiguration
    {
        public static List<Scope> Scopes => new List<Scope>
        {
            new Scope
            {
                Name = "firstScope",
                IsExposedInConfigurationEdp = true,
            }
        };

        public static List<Client> Clients = new List<Client>
        {
            new Client
            {
                ClientId = "firstClient",
                ClientSecret = "password",
                Scopes = new List<ClientScope>
                {
                    "firstScope"
                },
                GrantTypes = new string[] { ClientCredentialsHandler.GRANT_TYPE },
                TokenExpirationTimeInSeconds = 3600
            }
        };
    }
}
