// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Website.Resources;

namespace SimpleIdServer.IdServer.Website.Stores.RealmStore
{
    public class RealmEffects
    {
        private readonly IDbContextFactory<StoreDbContext> _factory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public RealmEffects(IDbContextFactory<StoreDbContext> factory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage protectedSessionStorage)
        {
            _factory = factory;
            _options = options.Value;
            _sessionStorage = protectedSessionStorage;
        }


        [EffectMethod]
        public async Task Handle(GetAllRealmAction action, IDispatcher dispatcher)
        {
            using (var dbContext = _factory.CreateDbContext())
            {
                IQueryable<Domains.Realm> query = dbContext.Realms.AsNoTracking();
                if (!_options.IsReamEnabled) query = query.Where(q => q.Name == SimpleIdServer.IdServer.Constants.DefaultRealm);
                IEnumerable<Domains.Realm> realms = await query.ToListAsync();
                dispatcher.Dispatch(new GetAllRealmSuccessAction { Realms = realms });
            }
        }

        [EffectMethod]
        public async Task Handle(AddRealmAction action, IDispatcher dispatcher)
        {
            if(!_options.IsReamEnabled)
            {
                dispatcher.Dispatch(new AddRealmFailureAction { ErrorMessage = Global.CannotAddRealmBecauseOptionIsDisabled });
                return;
            }

            using (var dbContext = _factory.CreateDbContext())
            {
                if (await dbContext.Realms.AsNoTracking().AnyAsync(r => r.Name == action.Name))
                {
                    var act = new AddRealmFailureAction { ErrorMessage = string.Format(Global.RealmExists, action.Name) };
                    dispatcher.Dispatch(act);
                    return;
                }

                var realm = new Domains.Realm { Name = action.Name, Description = action.Description, CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow };
                dbContext.Realms.Add(realm);
                var users = await dbContext.Users.Include(u => u.Realms).Where(u => WebsiteConfiguration.StandardUsers.Contains(u.Name)).ToListAsync();
                var clients = await dbContext.Clients.Include(c => c.Realms).Where(c => WebsiteConfiguration.StandardClients.Contains(c.ClientId)).ToListAsync();
                var scopes = await dbContext.Scopes.Include(s => s.Realms).Where(s => WebsiteConfiguration.StandardScopes.Contains(s.Name)).ToListAsync();
                var keys = await dbContext.SerializedFileKeys.Include(s => s.Realms).Where(s => s.Realms.Any(r => r.Name == Constants.DefaultRealm)).ToListAsync();
                var acrs = await dbContext.Acrs.Include(a => a.Realms).ToListAsync();
                var certificateAuthorities = await dbContext.CertificateAuthorities.Include(s => s.Realms).Where(s => s.Realms.Any(r => r.Name == Constants.DefaultRealm)).ToListAsync();
                foreach (var user in users)
                    user.Realms.Add(new RealmUser { RealmsName = action.Name });

                foreach (var client in clients)
                    client.Realms.Add(realm);

                foreach (var scope in scopes)
                    scope.Realms.Add(realm);

                foreach (var acr in acrs)
                    acr.Realms.Add(realm);

                foreach (var key in keys)
                    key.Realms.Add(realm);

                foreach (var certificateAuthority in certificateAuthorities)
                    certificateAuthority.Realms.Add(realm);

                await dbContext.SaveChangesAsync();
            }

            dispatcher.Dispatch(new AddRealmSuccessAction
            {
                Description = action.Description,
                Name = action.Name
            });
            dispatcher.Dispatch(new SelectRealmAction
            {
                Realm = action.Name
            });
        }

        [EffectMethod]
        public async Task Handle(SelectRealmAction action, IDispatcher dispatcher)
        {
            await _sessionStorage.SetAsync("realm", action.Realm);
            dispatcher.Dispatch(new SelectRealmSuccessAction { Realm = action.Realm });
        }

        [EffectMethod]
        public async Task Handle(GetActiveRealmAction action, IDispatcher dispatcher)
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            dispatcher.Dispatch(new GetActiveSuccessRealmAction { Realm = realmStr });
        }
    }

    public class GetActiveRealmAction
    {

    }

    public class GetActiveSuccessRealmAction
    {
        public string Realm { get; set; }
    }

    public class GetAllRealmAction
    {
        public IEnumerable<Domains.Realm> Realms { get; set; }
    }

    public class GetAllRealmSuccessAction
    {
        public IEnumerable<Domains.Realm> Realms { get; set; }
    }

    public class SelectRealmAction
    {
        public string Realm { get; set; }
    }

    public class SelectRealmSuccessAction
    {
        public string Realm { get; set; }
    }

    public class AddRealmAction
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class AddRealmFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class AddRealmSuccessAction
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
