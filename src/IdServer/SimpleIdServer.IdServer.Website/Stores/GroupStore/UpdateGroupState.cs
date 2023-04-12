// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.GroupStore
{
    [FeatureState]
    public record UpdateGroupState
    {
        public UpdateGroupState()
        {

        }

        public bool IsUpdating { get; set; } = false;
        public string ErrorMessage { get; set; } = null;
    }
}
