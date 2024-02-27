// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ApiResourceStore
{
    public class ApiResourceReducers
    {
        #region SearchApiResourcesState

        [ReducerMethod]
        public static SearchApiResourcesState ReduceSearchApiResourcesAction(SearchApiResourcesState state, SearchApiResourcesAction act) => new(isLoading: true, apiResources: new List<ApiResource>());

        [ReducerMethod]
        public static SearchApiResourcesState ReduceSearchApiResourcesSuccessAction(SearchApiResourcesState state, SearchApiResourcesSuccessAction act)
        {
            var apiResources = act.ApiResources.Select(c => new SelectableApiResource(c)).ToList();
            foreach (var apiResource in apiResources)
                apiResource.IsSelected = act.SelectedApiResources.Contains(apiResource.Value.Name);

            return state with
            {
                IsLoading = false,
                ApiResources = apiResources,
                Count = act.Count
            };
        }

        [ReducerMethod]
        public static SearchApiResourcesState ReduceAddApiResourceSuccessAction(SearchApiResourcesState state, AddApiResourceSuccessAction act)
        {
            var apiResources = state.ApiResources.ToList();
            var newApiResource = new ApiResource { Id = act.Id, CreateDateTime = DateTime.Now, UpdateDateTime = DateTime.Now, Name = act.Name, Description = act.Description, Audience = act.Audience };
            apiResources.Add(new SelectableApiResource(newApiResource) { IsNew = true });
            return state with
            {
                ApiResources = apiResources,
                Count = apiResources.Count()
            };
        }

        [ReducerMethod]
        public static SearchApiResourcesState ReduceRemoveSelectedApiResourcesSuccessAction(SearchApiResourcesState state, RemoveSelectedApiResourcesSuccessAction act)
        {
            var apiResources = state.ApiResources.ToList();
            apiResources = apiResources.Where(r => !act.ResourceIds.Contains(r.Value.Id)).ToList();
            return state with
            {
                ApiResources = apiResources,
                Count = apiResources.Count()
            };
        }

        [ReducerMethod]
        public static SearchApiResourcesState ReduceToggleApiResourceSelectionAction(SearchApiResourcesState state, ToggleApiResourceSelectionAction act)
        {
            var resources = state.ApiResources?.ToList();
            if (resources == null) return state;
            var selectedResource = resources.Single(c => c.Value.Name == act.ResourceName);
            selectedResource.IsSelected = act.IsSelected;
            return state with
            {
                ApiResources = resources
            };
        }

        [ReducerMethod]
        public static SearchApiResourcesState ReduceToggleAllApiResourceSelectionAction(SearchApiResourcesState state, ToggleAllApiResourceSelectionAction act)
        {
            var resources = state.ApiResources?.ToList();
            if (resources == null) return state;
            foreach (var resource in resources) resource.IsSelected = act.IsSelected;
            return state with
            {
                ApiResources = resources
            };
        }

        #endregion

        #region AddClientState

        [ReducerMethod]
        public static AddApiResourceState ReduceAddApiResourceAction(AddApiResourceState state, AddApiResourceAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddApiResourceState ReduceAddApiResourceSuccessAction(AddApiResourceState state, AddApiResourceSuccessAction act) => new(isAdding: false, errorMessage: null);

        [ReducerMethod]
        public static AddApiResourceState ReduceAddApiResourceSuccessAction(AddApiResourceState state, AddApiResourceFailureAction act) => new(isAdding: false, errorMessage: act.ErrorMessage);

        #endregion
    }
}
