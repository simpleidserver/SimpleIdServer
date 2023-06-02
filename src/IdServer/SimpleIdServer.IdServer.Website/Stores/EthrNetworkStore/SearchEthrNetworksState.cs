// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.EthrNetworkStore
{
    [FeatureState]
    public record SearchEthrNetworksState
    {
        public SearchEthrNetworksState()
        {

        }

        public SearchEthrNetworksState(bool isLoading, IEnumerable<NetworkConfiguration> networks, int nbNetworks)
        {
            Networks = networks.Select(g => new SelectableEthrNetwork(g));
            IsLoading = isLoading;
            Count = nbNetworks;
        }

        public IEnumerable<SelectableEthrNetwork> Networks { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = true;
    }

    public class SelectableEthrNetwork
    {
        public SelectableEthrNetwork(NetworkConfiguration networkConfiguration)
        {
            Value = networkConfiguration;
        }

        public bool IsSelected { get; set; }
        public bool IsNew { get; set; }
        public NetworkConfiguration Value { get; set; }
    }
}
