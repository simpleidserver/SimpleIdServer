// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.CredentialTemplateStore
{
    [FeatureState]
    public record CredentialTemplateDisplayState
    {
        public CredentialTemplateDisplayState() { }

        public CredentialTemplateDisplayState(bool isLoading, IEnumerable<CredentialTemplateDisplay> credentialTemplateDisplays, int nb)
        {
            CredentialTemplateDisplays = credentialTemplateDisplays.Select(c => new SelectableCredentialTemplateDisplay(c));
            Count = nb;
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableCredentialTemplateDisplay>? CredentialTemplateDisplays { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = true;
    }

    public class SelectableCredentialTemplateDisplay
    {
        public SelectableCredentialTemplateDisplay(CredentialTemplateDisplay value)
        {
            Value = value;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public CredentialTemplateDisplay Value { get; set; }
    }
}
