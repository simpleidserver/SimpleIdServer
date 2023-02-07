// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore
{
    [FeatureState]
    public record UserCredentialsState
    {
        public UserCredentialsState() { }

        public UserCredentialsState(bool isLoading, IEnumerable<UserCredential> userCredentials)
        {
            UserCredentials = userCredentials;
            Count = userCredentials.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<UserCredential>? UserCredentials { get; set; } = new List<UserCredential>();
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = false;
    }
}
