using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.RealmStore;

[FeatureState]
public record class RealmRoleState
{
    public RealmRoleState() { }

    public RealmRoleState(bool isLoading, RealmRole realmRole)
    {
        RealmRole = realmRole;
        IsLoading = isLoading;
    }

    public RealmRole RealmRole { get; set; }
    public bool IsLoading { get; set; } = true;
}
