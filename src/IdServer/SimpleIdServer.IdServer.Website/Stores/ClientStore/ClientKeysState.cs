// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    [FeatureState]
    public record ClientKeysState
    {
        public ClientKeysState() { }

        public ClientKeysState(bool isLoading, IEnumerable<ClientJsonWebKey> keys)
        {
            Keys = keys.Select(c => new SelectableClientKey(c));
            Count = keys.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableClientKey>? Keys { get; set; } = new List<SelectableClientKey>();
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = false;
    }

    public class SelectableClientKey
    {
        public SelectableClientKey(ClientJsonWebKey key)
        {
            Value = key;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public ClientJsonWebKey Value { get; set; }
    }

}
