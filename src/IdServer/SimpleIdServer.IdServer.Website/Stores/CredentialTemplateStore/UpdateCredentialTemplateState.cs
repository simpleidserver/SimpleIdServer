// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.CredentialTemplateStore
{
    [FeatureState]
    public record UpdateCredentialTemplateState
    {
        public UpdateCredentialTemplateState()
        {

        }


        public UpdateCredentialTemplateState(bool isLoading, string errorMessage)
        {
            IsLoading = isLoading;
            ErrorMessage = errorMessage;
        }

        public bool IsLoading { get; set; }
        public string ErrorMessage { get; set; }
    }
}
