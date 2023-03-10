// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Website.Resources;

namespace SimpleIdServer.IdServer.Website.Stores.RealmStore
{
    public class RealmEffects
    {
        private readonly IRealmRepository _realmRepository;
        private readonly DbContextOptions<StoreDbContext> _options;

        public RealmEffects(IRealmRepository realmRepository, DbContextOptions<StoreDbContext> options)
        {
            _realmRepository = realmRepository;
            _options = options;
        }


        [EffectMethod]
        public async Task Handle(GetAllRealmAction action, IDispatcher dispatcher)
        {
            IEnumerable<Realm> realms = await _realmRepository.Query().AsNoTracking().ToListAsync();
            dispatcher.Dispatch(new GetAllRealmSuccessAction { Realms = realms});
        }

        [EffectMethod]
        public async Task Handle(AddRealmAction action, IDispatcher dispatcher)
        {
            if(await _realmRepository.Query().AsNoTracking().AnyAsync(r => r.Name == action.Name))
            {
                var act = new AddRealmFailureAction { ErrorMessage = string.Format(Global.RealmExists, action.Name) };
                dispatcher.Dispatch(act);
                return;
            }

            using(var dbContext = new StoreDbContext(_options))
            {
                var realm = new Realm { Name = action.Name, Description = action.Description, CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow };
                dbContext.Realms.Add(realm);
                var users = await dbContext.Users.Include(u => u.Realms).Where(u => WebsiteConfiguration.StandardUsers.Contains(u.Name)).ToListAsync();
                var clients = await dbContext.Clients.Include(c => c.Realms).Where(c => WebsiteConfiguration.StandardClients.Contains(c.ClientId)).ToListAsync();
                var scopes = await dbContext.Scopes.Include(s => s.Realms).Where(s => WebsiteConfiguration.StandardScopes.Contains(s.Name)).ToListAsync();
                var acrs = await dbContext.Acrs.Include(a => a.Realms).ToListAsync();
                foreach (var user in users)
                    user.Realms.Add(realm);

                foreach (var client in clients)
                    client.Realms.Add(realm);

                foreach (var scope in scopes)
                    scope.Realms.Add(realm);

                foreach(var acr in acrs)
                    acr.Realms.Add(realm);

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
    }

    public class GetAllRealmAction
    {
        public IEnumerable<Realm> Realms { get; set; }
    }

    public class GetAllRealmSuccessAction
    {
        public IEnumerable<Realm> Realms { get; set; }
    }

    public class SelectRealmAction
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
