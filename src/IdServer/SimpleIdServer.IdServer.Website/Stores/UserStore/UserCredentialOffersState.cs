// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore
{
    [FeatureState]
    public record UserCredentialOffersState
    {       
        public UserCredentialOffersState() { }

        public UserCredentialOffersState(bool isLoading, IEnumerable<UserCredentialOffer> credentialOffers)
        {
            CredentialOffers = credentialOffers.Select(c => new SelectableUserCredentialOffer(c));
            Count = credentialOffers.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableUserCredentialOffer>? CredentialOffers { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = true;
    }

    public class SelectableUserCredentialOffer
    {
        public SelectableUserCredentialOffer(UserCredentialOffer value)
        {
            Value = value;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public UserCredentialOffer Value { get; set; }
    }
}
