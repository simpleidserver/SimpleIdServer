// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;
using static System.Formats.Asn1.AsnWriter;

namespace SimpleIdServer.IdServer.Website.Stores.ResourceStore
{
    public class ResourceReducers
    {
        #region SearchResourcesState

        [ReducerMethod]
        public static SearchResourcesState ReduceSearchResourcesAction(SearchResourcesState state, SearchResourcesAction act) => new(isLoading: true, scopes: new List<Domains.Scope>());

        [ReducerMethod]
        public static SearchResourcesState ReduceSearchResourcesSuccessAction(SearchResourcesState state, SearchResourcesSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                Scopes = act.Scopes.Select(c => new SelectableResource(c)),
                Count = act.Scopes.Count()
            };
        }

        [ReducerMethod]
        public static SearchResourcesState ReduceToggleResourceSelectionAction(SearchResourcesState state, ToggleResourceSelectionAction act)
        {
            var scopes = state.Scopes?.ToList();
            if (scopes == null) return state;
            var selectedScope = scopes.Single(c => c.Value.Name == act.ResourceName);
            selectedScope.IsSelected = act.IsSelected;
            return state with
            {
                Scopes = scopes
            };
        }

        [ReducerMethod]
        public static SearchResourcesState ReduceToggleAllUserSelectionAction(SearchResourcesState state, ToggleAllResourceSelectionAction act)
        {
            var scopes = state.Scopes?.ToList();
            if (scopes == null) return state;
            foreach (var scope in scopes) scope.IsSelected = act.IsSelected;
            return state with
            {
                Scopes = scopes
            };
        }

        [ReducerMethod]
        public static SearchResourcesState ReduceRemoveSelectedResourcesAction(SearchResourcesState state, RemoveSelectedResourcesAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchResourcesState ReduceRemoveSelectedResourcesSuccessAction(SearchResourcesState state, RemoveSelectedResourcesSuccessAction act)
        {
            var scopes = state.Scopes?.ToList();
            if (scopes == null) return state;
            scopes = scopes.Where(s => !act.ResourceNames.Contains(s.Value.Name)).ToList();
            return state with
            {
                Scopes = scopes,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static SearchResourcesState ReduceAddResourceSuccessAction(SearchResourcesState state, AddResourceSuccessAction act)
        {
            var scopes = state.Scopes?.ToList();
            scopes.Add(new SelectableResource(new Domains.Scope
            {
                Name = act.Name,
                Description = act.Description,
                IsExposedInConfigurationEdp = act.IsExposedInConfigurationEdp,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            })
            {
                IsNew = true
            });
            return state with
            {
                Scopes = scopes
            };
        }

        #endregion

        #region AddResourceState

        [ReducerMethod]
        public static AddResourceState ReduceAddIdentityResourceAction(AddResourceState state, AddIdentityResourceAction act) => state with
        {
            IsAdding = true
        };

        [ReducerMethod]
        public static AddResourceState ReduceAddResourceFailureAction(AddResourceState state, AddResourceFailureAction act) => state with
        {
            IsAdding = false,
            ErrorMessage = act.ErrorMessage
        };

        [ReducerMethod]
        public static AddResourceState ReduceAddResourceSuccessAction(AddResourceState state, AddResourceSuccessAction act) => state with
        {
            IsAdding = false
        };

        #endregion
    }
}
