using SimpleIdServer.OpenidFederation.Domains;

namespace SimpleIdServer.Federation.Ta.Startup;

public class TaConfiguration
{
    public static List<FederationEntity> FederationEntities = new List<FederationEntity>
    {
        new FederationEntity
        {
            Id = Guid.NewGuid().ToString(),
            Sub = "http://localhost:7001",
            Realm = string.Empty,
            IsSubordinate = true
        }
    };
}