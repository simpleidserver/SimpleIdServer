using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.RealmStore;

[FeatureState]
public record RealmRolesState
{
    public RealmRolesState() { }

    public RealmRolesState(bool isLoading, IEnumerable<RealmRole> realmRoles)
    {
        RealmRoles = realmRoles.Select(c => new SelectableRealmRole(c));
        Count = realmRoles.Count();
        IsLoading = isLoading;
    }

    public IEnumerable<SelectableRealmRole>? RealmRoles { get; set; }
    public int Count { get; set; } = 0;
    public bool IsLoading { get; set; } = true;
}

public class SelectableRealmRole
{
    public SelectableRealmRole(RealmRole realmRole)
    {
        Value = realmRole;
    }

    public bool IsSelected { get; set; } = false;
    public bool IsNew { get; set; } = false;
    public RealmRole Value { get; set; }
}