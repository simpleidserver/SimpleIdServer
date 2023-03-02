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

        #endregion

        #region IdProviderState

        [ReducerMethod]
        public static IdProviderState ReduceGetIdProviderAction(IdProviderState state, GetIdProviderAction act) => new IdProviderState(isLoading: true, null);

        [ReducerMethod]
        public static IdProviderState ReduceGetIdProviderFailureAction(IdProviderState state, GetIdProviderFailureAction act) => new IdProviderState(isLoading: false, null);

        [ReducerMethod]
        public static IdProviderState ReduceGetIdProviderSuccessAction(IdProviderState state, GetIdProviderSuccessAction act) => new IdProviderState(isLoading: false, act.IdProvider);

        #endregion

        #region IdProviderDefsState

        [ReducerMethod]
        public static IdProviderDefsState ReduceGetIdProviderAction(IdProviderDefsState state, GetIdProviderDefsAction act) => new IdProviderDefsState(new List<AuthenticationSchemeProviderDefinition>(), true);

        [ReducerMethod]
        public static IdProviderDefsState ReduceGetIdProviderDefsSuccessAction(IdProviderDefsState state, GetIdProviderDefsSuccessAction act) => new IdProviderDefsState(act.AuthProviderDefinitions.ToList(), false);

        #endregion
    }
}
