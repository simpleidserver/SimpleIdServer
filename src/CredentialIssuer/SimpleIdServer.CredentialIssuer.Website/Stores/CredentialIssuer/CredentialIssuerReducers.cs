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
            CredentialConfigurations = action.CredentialConfigurations.ToList()
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationsState ReduceDeleteCredentialConfigurationAction(CredentialConfigurationsState state, DeleteCredentialConfigurationAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }


    [ReducerMethod]
    public static CredentialConfigurationsState ReduceDeleteCredentialConfigurationSuccessAction(CredentialConfigurationsState state, DeleteCredentialConfigurationSuccessAction action)
    {
        var credentialConfigurations = state.CredentialConfigurations;
        var credentialConfiguration = credentialConfigurations.Single(c => c.Id == action.Id);
        credentialConfigurations.Remove(credentialConfiguration);
        return state with
        {
            IsLoading = false,
            CredentialConfigurations = credentialConfigurations
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationsState ReduceAddCredentialConfigurationAction(CredentialConfigurationsState state, AddCredentialConfigurationAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationsState ReduceAddCredentialConfigurationSuccessAction(CredentialConfigurationsState state, AddCredentialConfigurationSuccessAction action)
    {
        var credentialConfigurations = state.CredentialConfigurations;
        credentialConfigurations.Add(action.CredentialConfiguration);
        return state with
        {
            IsLoading = false,
            CredentialConfigurations = credentialConfigurations
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationsState ReduceAddCredentialConfigurationFailureAction(CredentialConfigurationsState state, AddCredentialConfigurationFailureAction action)
    {
        return state with
        {
            IsLoading = false
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

    [ReducerMethod]
    public static CredentialConfigurationState ReduceAddCredentialClaimAction(CredentialConfigurationState state, AddCredentialClaimAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceAddCredentialClaimSuccessAction(CredentialConfigurationState state, AddCredentialClaimSuccessAction action)
    {
        var credConfiguration = state.CredentialConfiguration;
        credConfiguration.Claims.Add(action.Claim);
        return state with
        {
            IsLoading = false,
            CredentialConfiguration = credConfiguration
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceAddCredentialClaimFailureAction(CredentialConfigurationState state, AddCredentialClaimFailureAction action)
    {
        return state with
        {
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceDeleteCredentialClaimAction(CredentialConfigurationState state, DeleteCredentialClaimAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceDeleteCredentialClaimSuccessAction(CredentialConfigurationState state, DeleteCredentialClaimSuccessAction action)
    {
        var credConfiguration = state.CredentialConfiguration;
        var claim = credConfiguration.Claims.Single(c => c.Id == action.ClaimId);
        credConfiguration.Claims.Remove(claim);
        return state with
        {
            IsLoading = false,
            CredentialConfiguration = credConfiguration
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceAddCredentialClaimTranslationAction(CredentialConfigurationState state, AddCredentialClaimTranslationAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceAddCredentialClaimTranslationSuccessAction(CredentialConfigurationState state, AddCredentialClaimTranslationSuccessAction action)
    {
        var credentialConfiguration = state.CredentialConfiguration;
        var claim = credentialConfiguration.Claims.Single(c => c.Id == action.ClaimId);
        claim.Translations.Add(action.Translation);
        return state with
        {
            IsLoading = false,
            CredentialConfiguration = credentialConfiguration
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceAddCredentialClaimTranslationFailureAction(CredentialConfigurationState state, AddCredentialClaimTranslationFailureAction action)
    {
        return state with
        {
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceUpdateCredentialClaimTranslationAction(CredentialConfigurationState state, UpdateCredentialClaimTranslationAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceUpdateCredentialClaimTranslationSuccessAction(CredentialConfigurationState state, UpdateCredentialClaimTranslationSuccessAction action)
    {
        var credentialConfiguration = state.CredentialConfiguration;
        var claim = credentialConfiguration.Claims.Single(c => c.Id == action.ClaimId);
        var tr = claim.Translations.Single(c => c.Id == action.TranslationId);
        tr.Name = action.Name;
        tr.Locale = action.Locale;
        return state with
        {
            IsLoading = false,
            CredentialConfiguration = credentialConfiguration
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceUpdateCredentialClaimTranslationFailureAction(CredentialConfigurationState state, UpdateCredentialClaimTranslationFailureAction action)
    {
        return state with
        {
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceDeleteCredentialClaimTranslationAction(CredentialConfigurationState state, DeleteCredentialClaimTranslationAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialConfigurationState ReduceDeleteCredentialClaimTranslationSuccessAction(CredentialConfigurationState state, DeleteCredentialClaimTranslationSuccessAction action)
    {
        var credentialConfiguration = state.CredentialConfiguration;
        var claim = credentialConfiguration.Claims.Single(c => c.Id == action.ClaimId);
        var tr = claim.Translations.Single(c => c.Id == action.TranslationId);
        claim.Translations.Remove(tr);
        return state with
        {
            IsLoading = false,
            CredentialConfiguration = credentialConfiguration
        };
    }

    #endregion

    #region CredentialsState

    [ReducerMethod]
    public static CredentialsState ReduceIssueCredentialInstanceAction(CredentialsState state, IssueCredentialInstanceAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialsState ReduceIssueCredentialInstanceSuccessAction(CredentialsState state, IssueCredentialInstanceSuccessAction action)
    {
        var credentials = state.Credentials;
        credentials.Add(action.Credential);
        return state with
        {
            IsLoading = false,
            Credentials = credentials
        };
    }

    [ReducerMethod]
    public static CredentialsState ReduceIssueCredentialInstanceFailureAction(CredentialsState state, IssueCredentialInstanceFailureAction action)
    {
        return state with
        {
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static CredentialsState ReduceGetCredentialInstancesAction(CredentialsState state, GetCredentialInstancesAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialsState ReduceGetCredentialInstancesSuccessAction(CredentialsState state, GetCredentialInstancesSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            Credentials = action.Credentials
        };
    }

    [ReducerMethod]
    public static CredentialsState ReduceRemoveCredentialInstanceAction(CredentialsState state, RemoveCredentialInstanceAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static CredentialsState ReduceRemoveCredentialInstanceSuccessAction(CredentialsState state, RemoveCredentialInstanceSuccessAction action)
    {
        var credential = state.Credentials.Single(c => c.Id == action.Id);
        state.Credentials.Remove(credential);
        return state with
        {
            IsLoading = false,
            Credentials = state.Credentials
        };
    }

    #endregion

    #region DeferredCredentialsState

    [ReducerMethod]
    public static DeferredCredentialsState ReduceSearchDeferredCredentialsAction(DeferredCredentialsState state, SearchDeferredCredentialsAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static DeferredCredentialsState ReduceSearchDeferredCredentialsSuccessAction(DeferredCredentialsState state, SearchDeferredCredentialsSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            DeferredCredentials = action.DeferredCredentials
        };
    }

    #endregion

    #region DeferredCredentialState

    [ReducerMethod]
    public static DeferredCredentialState ReduceGetDeferredCredentialAction(DeferredCredentialState state, GetDeferredCredentialAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static DeferredCredentialState ReduceGetDeferredCredentialSuccessAction(DeferredCredentialState state, GetDeferredCredentialSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            DeferredCredential = action.DeferredCredential
        };
    }

    [ReducerMethod]
    public static DeferredCredentialState ReduceIssueDeferredCredentialAction(DeferredCredentialState state, IssueDeferredCredentialAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static DeferredCredentialState ReduceIssueDeferredCredentialSuccessAction(DeferredCredentialState state, IssueDeferredCredentialSuccessAction action)
    {
        var deferredCredential = state.DeferredCredential;
        deferredCredential.Claims = action.Claims.Select(kvp => new Domains.DeferredCredentialClaim
        {
            Name = kvp.Key,
            Value = kvp.Value
        }).ToList();
        deferredCredential.Status = Domains.DeferredCredentialStatus.ISSUED;
        return state with
        {
            IsLoading = false,
            DeferredCredential = deferredCredential
        };
    }

    #endregion
}
