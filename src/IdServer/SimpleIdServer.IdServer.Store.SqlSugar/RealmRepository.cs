// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class RealmRepository : IRealmRepository
{
    private readonly DbContext _dbContext;
    
    public RealmRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Realm realm)
    {
        _dbContext.Client.Insertable(realm).IgnoreColumns(
                nameof(Realm.Clients),
                nameof(Realm.Users),
                nameof(Realm.Scopes),
                nameof(Realm.AuthenticationContextClassReferences),
                nameof(Realm.AuthenticationSchemeProviders),
                nameof(Realm.ApiResources),
                nameof(Realm.SerializedFileKeys),
                nameof(Realm.CertificateAuthorities),
                nameof(Realm.IdentityProvisioningLst),
                nameof(Realm.Groups),
                nameof(Realm.RegistrationWorkflows),
                nameof(Realm.PresentationDefinitions)).ExecuteCommand();
    }

    public Task<Realm> Get(string name, CancellationToken cancellationToken)
        => GetRealms().FirstAsync(c => c.Name == name, cancellationToken);

    public Task<List<Realm>> GetAll(CancellationToken cancellationToken)
        => GetRealms().ToListAsync(cancellationToken);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => Task.FromResult(1);

    private ISugarQueryable<Realm> GetRealms()
        => _dbContext.Client.
            Queryable<Realm>()
            .IgnoreColumns(
                nameof(Realm.Clients),
                nameof(Realm.Users),
                nameof(Realm.Scopes),
                nameof(Realm.AuthenticationContextClassReferences),
                nameof(Realm.AuthenticationSchemeProviders),
                nameof(Realm.ApiResources),
                nameof(Realm.SerializedFileKeys),
                nameof(Realm.CertificateAuthorities),
                nameof(Realm.IdentityProvisioningLst),
                nameof(Realm.Groups),
                nameof(Realm.RegistrationWorkflows),
                nameof(Realm.PresentationDefinitions));
}