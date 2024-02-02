// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;


namespace SimpleIdServer.CredentialIssuer.Website.Stores.CredentialIssuer;

public static class CredentialIssuerReducers
{
    #region CredentialConfigurationsState

    [ReducerMethod]
    public static CredentialConfigurationsState ReduceGetCredentialConfigurationsAction(ConfigurationDefinitionsState state, GetCredentialConfigurationsAction action) => new CredentialConfigurationsState { IsLoading = true };

    [ReducerMethod]
    public static CredentialConfigurationsState ReduceGetCredentialConfigurationsSuccessAction(CredentialConfigurationsState state, GetCredentialConfigurationsSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            CredentialConfigurations = action.CredentialConfigurations
        };
    }

    #endregion
}
