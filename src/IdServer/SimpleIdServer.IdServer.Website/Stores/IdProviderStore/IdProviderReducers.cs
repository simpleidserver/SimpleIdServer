// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.IdProviderStore
{
    public static class IdProviderReducers
    {
        #region SearchIdProvidersState

        [ReducerMethod]
        public static SearchIdProvidersState ReduceGetIdProvidersAction(SearchIdProvidersState state, SearchIdProvidersAction act) => new(idProviders: new List<AuthenticationSchemeProvider>(), true);

        [ReducerMethod]
        public static SearchIdProvidersState ReduceSearchIdProvidersSuccessAction(SearchIdProvidersState state, SearchIdProvidersSuccessAction act) => new(idProviders: act.IdProviders, false)
        {
            Count = act.IdProviders.Count
        };

        [ReducerMethod]
        public static SearchIdProvidersState ReduceDeleteIdProviderSuccessAction(SearchIdProvidersState state, RemoveSelectedIdProvidersSuccessAction act)
        {
            var idProviders = state.IdProviders.ToList();
            idProviders = idProviders.Where(p => !act.Ids.Contains(p.Value.Name)).ToList();
            return state with
            {
                IdProviders = idProviders,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static SearchIdProvidersState ReduceToggleIdProviderSelectionAction(SearchIdProvidersState state, ToggleIdProviderSelectionAction act)
        {
            var idProviders = state.IdProviders.ToList();
            var selectedIdProvider = idProviders.Single(p => p.Value.Name == act.Id).IsSelected = act.IsSelected;
            return state with
            {
                IdProviders = idProviders
            };
        }

        [ReducerMethod]
        public static SearchIdProvidersState ReduceToggleAllIdProvidersSelectionAction(SearchIdProvidersState state, ToggleAllIdProvidersSelectionAction act)
        {
            var idProviders = state.IdProviders.ToList();
            foreach (var idProvider in idProviders)
                idProvider.IsSelected = act.IsSelected;
            return state with
            {
                IdProviders = idProviders
            };
        }

        [ReducerMethod]
        public static SearchIdProvidersState ReduceAddIdProviderSuccessAction(SearchIdProvidersState state, AddIdProviderSuccessAction act)
        {
            var idProviders = state.IdProviders.ToList();
            idProviders.Add(new SelectableIdProvider(new AuthenticationSchemeProvider { CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow, Description = act.Description, DisplayName = act.DisplayName, Name = act.Name })
            {
                IsNew = true
            });
            return state with
            {
                IdProviders = idProviders
            };
        }

        [ReducerMethod]
        public static SearchIdProvidersState ReduceUpdateIdProviderDetailsAction(SearchIdProvidersState state, UpdateIdProviderDetailsAction act)
        {
            var idProviders = state.IdProviders.ToList();
            var idProvider = idProviders.First(i => i.Value.Name == act.Name);
            idProvider.Value.Description = act.Description;
            idProvider.Value.DisplayName = act.DisplayName;
            idProvider.Value.UpdateDateTime = DateTime.UtcNow;
            return state with
            {
                IdProviders = idProviders
            };
        }

        #endregion

        #region IdProviderState

        [ReducerMethod]
        public static IdProviderState ReduceGetIdProviderAction(IdProviderState state, GetIdProviderAction act) => new IdProviderState(isLoading: true, null);

        [ReducerMethod]
        public static IdProviderState ReduceGetIdProviderFailureAction(IdProviderState state, GetIdProviderFailureAction act) => new IdProviderState(isLoading: false, null);

        [ReducerMethod]
        public static IdProviderState ReduceGetIdProviderSuccessAction(IdProviderState state, GetIdProviderSuccessAction act) => new IdProviderState(isLoading: false, act.IdProvider);

        [ReducerMethod]
        public static IdProviderState ReduceUpdateIdProviderDetailsAction(IdProviderState state, UpdateIdProviderDetailsAction act)
        {
            var provider = state.Provider;
            provider.DisplayName = act.DisplayName;
            provider.Description = act.Description;
            provider.UpdateDateTime = DateTime.UtcNow;
            return state with
            {
                Provider = provider
            };
        }

        [ReducerMethod]
        public static IdProviderState ReduceUpdateAuthenticationSchemeProviderPropertiesAction(IdProviderState state, UpdateAuthenticationSchemeProviderPropertiesAction act)
        {
            var provider = state.Provider;
            provider.UpdateDateTime = DateTime.UtcNow;
            provider.Properties.Clear();
            foreach (var property in act.Properties)
                provider.Properties.Add(new AuthenticationSchemeProviderProperty
                {
                    PropertyName = property.PropertyName,
                    Value = property.Value
                });
            return state with
            {
                Provider = provider
            };
        }

        #endregion

        #region IdProviderDefsState

        [ReducerMethod]
        public static IdProviderDefsState ReduceGetIdProviderAction(IdProviderDefsState state, GetIdProviderDefsAction act) => new IdProviderDefsState(new List<AuthenticationSchemeProviderDefinition>(), true);

        [ReducerMethod]
        public static IdProviderDefsState ReduceGetIdProviderDefsSuccessAction(IdProviderDefsState state, GetIdProviderDefsSuccessAction act) => new IdProviderDefsState(act.AuthProviderDefinitions.ToList(), false);

        #endregion

        #region UpdateIdProviderState

        [ReducerMethod]
        public static UpdateIdProviderState ReduceAddIdProviderAction(UpdateIdProviderState state, AddIdProviderAction act) => new UpdateIdProviderState(true, null);

        [ReducerMethod]
        public static UpdateIdProviderState ReduceAddIdProviderFailureAction(UpdateIdProviderState state, AddIdProviderFailureAction act) => new UpdateIdProviderState(false, act.ErrorMessage);

        [ReducerMethod]
        public static UpdateIdProviderState ReduceAddIdProviderSuccessAction(UpdateIdProviderState state, AddIdProviderSuccessAction act) => new UpdateIdProviderState(false, null);

        [ReducerMethod]
        public static UpdateIdProviderState ReduceUpdateIdProviderDetailsAction(UpdateIdProviderState state, UpdateIdProviderDetailsAction act)
        {
            return state with
            {
                IsUpdating = true,
                ErrorMessage = null
            };
        }

        [ReducerMethod]
        public static UpdateIdProviderState ReduceUpdateIdProviderDetailsSuccessAction(UpdateIdProviderState state, UpdateIdProviderDetailsSuccessAction act)
        {
            return state with
            {
                IsUpdating = false,
                ErrorMessage = null
            };
        }

        [ReducerMethod]
        public static UpdateIdProviderState ReduceUpdateAuthenticationSchemeProviderPropertiesAction(UpdateIdProviderState state, UpdateAuthenticationSchemeProviderPropertiesAction act)
        {
            return state with
            {
                IsUpdating = true,
                ErrorMessage = null
            };
        }

        [ReducerMethod]
        public static UpdateIdProviderState ReduceUpdateAuthenticationSchemeProviderPropertiesSuccessAction(UpdateIdProviderState state, UpdateAuthenticationSchemeProviderPropertiesSuccessAction act)
        {
            return state with
            {
                IsUpdating = false,
                ErrorMessage = null
            };
        }

        #endregion
    }
}
