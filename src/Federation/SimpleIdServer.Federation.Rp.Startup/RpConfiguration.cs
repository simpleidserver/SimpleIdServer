using SimpleIdServer.OpenidFederation.Domains;

namespace SimpleIdServer.Federation.Rp.Startup;

public class RpConfiguration
{
    public static List<FederationEntity> FederationEntities = new List<FederationEntity>
    {
        new FederationEntity
        {
            Id = Guid.NewGuid().ToString(),
            Sub = "http://localhost:7000",
            Realm = string.Empty,
            IsSubordinate = false
        }
    };
}