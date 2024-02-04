// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;


namespace SimpleIdServer.CredentialIssuer.Website.Stores.CredentialIssuer;

public static class CredentialIssuerReducers
{
    #region CredentialConfigurationsState

    [ReducerMethod]
    public static CredentialConfigurationsState ReduceGetCredentialConfigurationsAction(CredentialConfigurationsState state, GetCredentialConfigurationsAction action) => new CredentialConfigurationsState { IsLoading = true };

    [ReducerMethod]
    public static CredentialConfigurationsState ReduceGetCredentialConfigurationsSuccessAction(CredentialConfigurationsState state, GetCredentialConfigurationsSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            CredentialConfigurations = action.CredentialConfigurations.Select(c => new SelectableCredentialConfiguration(c)
            {
                IsNew = false,
                IsSelected = false
            }).ToList()
        };
    }

    #endregion

    #region CredentialConfigurationState

    [ReducerMethod]
    public static CredentialConfigurationState ReduceGetCredentialConfigurationAction(CredentialConfigurationState state, GetCredentialConfigurationAction action) => new CredentialConfigurationState { IsLoading = true };

    [ReducerMethod]
    public static CredentialConfigurationState ReduceGetCredentialConfigurationSuccessAction(CredentialConfigurationState state, GetCredentialConfigurationSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            CredentialConfiguration = action.Configuration
        };
    }

    #endregion
}
