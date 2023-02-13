// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    [FeatureState]
    public record ClientState
    {
        public ClientState() { }

        public ClientState(bool isLoading, Client? client)
        {
            IsLoading = isLoading;
            Client = client;
        }

        public Client? Client { get; set; } = new Client();
        public bool IsLoading { get; set; } = true;
    }
}
