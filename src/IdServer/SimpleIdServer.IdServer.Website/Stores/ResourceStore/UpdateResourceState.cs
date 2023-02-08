// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.ResourceStore
{
    [FeatureState]
    public record UpdateResourceState
    {
        public UpdateResourceState() { }

        public UpdateResourceState(bool isUpdating)
        {
            IsUpdating = isUpdating;
        }

        public bool IsUpdating { get; set; } = false;
    }
}
