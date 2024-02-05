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

    [ReducerMethod]
    public static CredentialConfigurationState ReduceUpdateCredentialDetailsAction(CredentialConfigurationState state, UpdateCredentialDetailsAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceUpdateCredentialDetailsSuccessAction(CredentialConfigurationState state, UpdateCredentialDetailsSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            CredentialConfiguration = action.CredentialConfiguration
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceUpdateCredentialDetailsErrorAction(CredentialConfigurationState state, UpdateCredentialDetailsErrorAction action)
    {
        return state with
        {
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceAddCredentialDisplayAction(CredentialConfigurationState state, AddCredentialDisplayAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceAddCredentialDisplaySuccessAction(CredentialConfigurationState state, AddCredentialDisplaySuccessAction action)
    {
        var credConf = state.CredentialConfiguration;
        credConf.Displays.Add(action.Display);
        return state with
        {
            IsLoading = false,
            CredentialConfiguration = credConf
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceAddCredentialDisplayErrorAction(CredentialConfigurationState state, AddCredentialDisplayErrorAction action)
    {
        return state with
        {
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceUpdateCredentialDisplayAction(CredentialConfigurationState state, UpdateCredentialDisplayAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceUpdateCredentialDisplaySuccessAction(CredentialConfigurationState state, UpdateCredentialDisplaySuccessAction action)
    {
        var credConf = state.CredentialConfiguration;
        var existingDisplay = credConf.Displays.Single(d => d.Id == action.DisplayId);
        existingDisplay.Description = action.Description;
        existingDisplay.BackgroundColor = action.BackgroundColor;
        existingDisplay.Locale = action.Locale;
        existingDisplay.Name = action.Name;
        existingDisplay.TextColor = action.TextColor;
        existingDisplay.LogoAltText = action.LogoAltText;
        existingDisplay.LogoUrl = action.LogoUrl;
        return state with
        {
            IsLoading = false,
            CredentialConfiguration = credConf
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceUpdateCredentialDisplayErrorAction(CredentialConfigurationState state, UpdateCredentialDisplayErrorAction action)
    {
        return state with
        {
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceDeleteCredentialDisplayAction(CredentialConfigurationState state, DeleteCredentialDisplayAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceDeleteCredentialDisplaySuccessAction(CredentialConfigurationState state, DeleteCredentialDisplaySuccessAction action)
    {
        var credConf = state.CredentialConfiguration;
        var existingDisplay = credConf.Displays.Single(d => d.Id == action.DisplayId);
        credConf.Displays.Remove(existingDisplay);
        return state with
        {
            IsLoading = false,
            CredentialConfiguration = credConf
        };
    }

    #endregion
}
