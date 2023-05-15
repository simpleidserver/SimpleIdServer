// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore
{
    [FeatureState]
    public record UserCredentialOfferState
    {
        public bool IsLoading { get; set; } = true;
        public string Picture { get; set; }
        public string Url { get; set; }
    }
}
