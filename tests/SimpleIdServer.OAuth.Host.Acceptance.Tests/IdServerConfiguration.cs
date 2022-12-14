using SimpleIdServer.Domains;
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
    }
}
