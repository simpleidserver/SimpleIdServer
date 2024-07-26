// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.RealmStore
{
    [FeatureState]
    public record RealmsState
    {
        public RealmsState() { }

        public RealmsState(bool isLoading, IEnumerable<Domains.Realm> realms)
        {
            IsLoading = isLoading;
            Realms = realms;
        }

        public bool IsLoading { get; set; } = true;
        public IEnumerable<Domains.Realm> Realms { get; set; }
    }
}
