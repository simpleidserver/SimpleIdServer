// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.RealmStore
{
    [FeatureState]
    public record RealmsState
    {
        public RealmsState() { }

        public RealmsState(bool isLoading, IEnumerable<Realm> realms)
        {
            IsLoading = isLoading;
            Realms = realms;
        }

        public bool IsLoading { get; set; } = true;
        public IEnumerable<Realm> Realms { get; set; }
        public string? ActiveRealm { get; set; } = SimpleIdServer.IdServer.Constants.DefaultRealm;
    }
}
