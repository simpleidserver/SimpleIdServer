// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.CredentialTemplateStore
{
    [FeatureState]
    public record CredentialTemplateState
    {
        public CredentialTemplateState() { }

        public CredentialTemplateState(bool isLoading, CredentialTemplate? credentialTemplate)
        {
            IsLoading = isLoading;
            CredentialTemplate = credentialTemplate;
        }

        public CredentialTemplate CredentialTemplate { get; set; } = new CredentialTemplate();
        public bool IsLoading { get; set; } = true;
    }
}