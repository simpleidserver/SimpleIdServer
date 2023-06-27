// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.StatisticStore
{
    [FeatureState]
    public record StatisticsState
    {
        public int NbUsers { get; set; }
        public int NbClients { get; set; }
        public int NbValidAuthentications { get; set; }
        public int NbInvalidAuthentications { get; set; }
        public bool IsLoading { get; set; } = true;
    }
}
