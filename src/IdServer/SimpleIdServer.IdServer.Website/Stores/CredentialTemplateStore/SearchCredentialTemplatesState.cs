// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.CredentialTemplateStore
{
    [FeatureState]
    public record SearchCredentialTemplatesState
    {
        public SearchCredentialTemplatesState() { }

        public SearchCredentialTemplatesState(bool isLoading, IEnumerable<CredentialTemplate> credentialTemplates, int nb)
        {
            CredentialTemplates = credentialTemplates.Select(c => new SelectableCredentialTemplate(c));
            Count = nb;
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableCredentialTemplate>? CredentialTemplates { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = true;
    }

    public class SelectableCredentialTemplate
    {

        public SelectableCredentialTemplate(CredentialTemplate value)
        {
            Value = value;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public CredentialTemplate Value { get; set; }
    }
}
