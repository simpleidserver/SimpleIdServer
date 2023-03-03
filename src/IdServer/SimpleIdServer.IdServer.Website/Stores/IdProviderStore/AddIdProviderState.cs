// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.IdProviderStore
{
    [FeatureState]
    public record AddIdProviderState
    {
        public AddIdProviderState()
        {

        }

        public AddIdProviderState(bool isAdding, string errorMesasge)
        {
            IsAdding = isAdding;
            ErrorMessage = errorMesasge;
        }

        public bool IsAdding { get; set; } = false;
        public string ErrorMessage { get; set; }
    }
}
