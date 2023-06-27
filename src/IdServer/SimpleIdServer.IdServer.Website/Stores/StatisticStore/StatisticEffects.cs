// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Events;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Store;

namespace SimpleIdServer.IdServer.Website.Stores.StatisticStore
{
    public class StatisticEffects
    {
        private readonly IDbContextFactory<StoreDbContext> _factory;
        private readonly ProtectedSessionStorage _sessionStorage;

        public StatisticEffects(IDbContextFactory<StoreDbContext> factory, ProtectedSessionStorage sessionStorage)
        {
            _factory= factory;
            _sessionStorage= sessionStorage;
        }

        [EffectMethod]
        public async Task Handle(GetStatisticsAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var currentDate = DateTime.UtcNow.Date;
                var nbUsers = await dbContext.Users.Include(u => u.Realms).CountAsync(u => u.Realms.Any(r => r.RealmsName == realm));
                var nbClients = await dbContext.Clients.Include(u => u.Realms).CountAsync(u => u.Realms.Any(r => r.Name == realm));
                var nbValidAuthentications = await dbContext.AuditEvents.CountAsync(e => e.CreateDateTime >= currentDate && e.EventName == nameof(UserLoginSuccessEvent) && e.Realm == realm);
                var nbInvalidAuthentications = await dbContext.AuditEvents.CountAsync(e => e.CreateDateTime >= currentDate && e.EventName == nameof(UserLoginFailureEvent) && e.Realm == realm);
                dispatcher.Dispatch(new GetStatisticsSuccessAction { NbClients = nbClients, NbUsers = nbUsers, NbInvalidAuthentications = nbInvalidAuthentications, NbValidAuthentications = nbValidAuthentications });
            }
        }

        private async Task<string> GetRealm()
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class GetStatisticsAction
    {

    }

    public class GetStatisticsSuccessAction
    {
        public int NbUsers { get; set; }
        public int NbClients { get; set; }
        public int NbValidAuthentications { get; set; }
        public int NbInvalidAuthentications { get; set; }
    }
}
