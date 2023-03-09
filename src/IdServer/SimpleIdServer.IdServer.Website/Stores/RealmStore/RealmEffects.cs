// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;

namespace SimpleIdServer.IdServer.Website.Stores.RealmStore
{
    public class RealmEffects
    {
        private readonly IRealmRepository _realmRepository;

        public RealmEffects(IRealmRepository realmRepository)
        {
            _realmRepository = realmRepository;
        }


        [EffectMethod]
        public async Task Handle(GetAllRealmAction action, IDispatcher dispatcher)
        {
            IEnumerable<Realm> realms = await _realmRepository.Query().AsNoTracking().ToListAsync();
            dispatcher.Dispatch(new GetAllRealmSuccessAction { Realms = realms});
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
}
