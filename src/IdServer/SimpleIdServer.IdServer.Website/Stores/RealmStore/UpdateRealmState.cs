// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.RealmStore
{
    [FeatureState]
    public record UpdateRealmState
    {
        public UpdateRealmState() { }

        public UpdateRealmState(bool isUpdating, string errorMessage)
        {
            IsUpdating = isUpdating;
            ErrorMessage = errorMessage;
        }

        public bool IsUpdating { get; set; } = false;
        public string ErrorMessage { get; set; }
    }
}
