// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Api.AuthenticationSchemeProviders;

namespace SimpleIdServer.IdServer.Website.Stores.IdProviderStore
{
    [FeatureState]
    public record IdProviderMappersState
    {
        public IdProviderMappersState() { }

        public IdProviderMappersState(ICollection<AuthenticationSchemeProviderMapperResult> mappers, bool isLoading)
        {
            Mappers = mappers?.Select(m => new SelectableAuthenticationSchemeProviderMapper(m));
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableAuthenticationSchemeProviderMapper> Mappers { get; set; } = new List<SelectableAuthenticationSchemeProviderMapper>();
        public bool IsLoading { get; set; } = true;
        public int Count { get; set; }
    }

    public class SelectableAuthenticationSchemeProviderMapper
    {
        public SelectableAuthenticationSchemeProviderMapper(AuthenticationSchemeProviderMapperResult value)
        {
            Value = value;
        }

        public AuthenticationSchemeProviderMapperResult Value { get; set; }
        public bool IsSelected { get; set; }
        public bool IsNew { get; set; }
    }
}
