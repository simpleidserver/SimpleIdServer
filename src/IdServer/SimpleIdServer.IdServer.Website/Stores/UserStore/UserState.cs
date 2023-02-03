// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore
{
    [FeatureState]
    public record UserState
    {
        public UserState() { }

        public UserState(bool isLoading, User? user)
        {
            IsLoading = isLoading;
            User = user;
        }

        public User? User { get; set; } = new User();
        public bool IsLoading { get; set; } = false;
    }
}
