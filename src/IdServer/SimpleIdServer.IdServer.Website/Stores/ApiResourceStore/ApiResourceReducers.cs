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
        public static ApiResourcesState ReduceSearchApiResourcesAction(ApiResourcesState state, SearchApiResourcesAction act) => new(
            isLoading: true, 
            availableApiResources: new List<ApiResource>(), 
            activeApiResources: new List<ApiResource>());

        [ReducerMethod]
        public static ApiResourcesState ReduceSearchApiResourcesSuccessAction(ApiResourcesState state, SearchApiResourcesSuccessAction act)
        {
            var allApiResources = act.ApiResources.Select(c => new SelectableApiResource(c)).ToList();
            var availableApiResources = allApiResources.Where(a => !act.SelectedApiResources.Contains(a.Value.Name));
            var activeApiResources = allApiResources.Where(a => act.SelectedApiResources.Contains(a.Value.Name));
            return state with
            {
                IsLoading = false,
                AvailableApiResources = availableApiResources,
                AvailableCount = availableApiResources.Count(),
                ActiveApiResources = activeApiResources,
                ActiveCount = activeApiResources.Count(),
            };
        }

        [ReducerMethod]
        public static ApiResourcesState ReduceAddApiResourceSuccessAction(ApiResourcesState state, AddApiResourceSuccessAction act)
        {
            var availableApiResources = state.AvailableApiResources.ToList();
            var newApiResource = new ApiResource { Id = act.Id, CreateDateTime = DateTime.Now, UpdateDateTime = DateTime.Now, Name = act.Name, Description = act.Description, Audience = act.Audience };
            availableApiResources.Add(new SelectableApiResource(newApiResource) { IsNew = true });
            return state with
            {
                AvailableApiResources = availableApiResources,
                AvailableCount = availableApiResources.Count(),
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static ApiResourcesState ReduceRemoveSelectedApiResourcesSuccessAction(ApiResourcesState state, RemoveSelectedApiResourcesSuccessAction act)
        {
            var availableApiResources = state.AvailableApiResources.ToList();
            availableApiResources = availableApiResources.Where(r => !act.ResourceIds.Contains(r.Value.Id)).ToList();
            foreach (var resource in availableApiResources) resource.IsSelected = false;
            return state with
            {
                AvailableApiResources = availableApiResources,
                AvailableCount = availableApiResources.Count,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static ApiResourcesState ReduceUpdateApiScopeResourcesSuccessAction(ApiResourcesState state, UpdateApiScopeResourcesSuccessAction act)
        {
            var activeApiResources = state.ActiveApiResources.ToList();
            var availableApiResources = state.AvailableApiResources.ToList();
            var removedApiResources = availableApiResources.Where(r => act.Resources.Contains(r.Value.Name)).ToList();
            activeApiResources.AddRange(removedApiResources);
            availableApiResources = availableApiResources.Where(r => !act.Resources.Contains(r.Value.Name)).ToList();
            foreach (var resource in activeApiResources) resource.IsSelected = false;
            foreach (var resource in availableApiResources) resource.IsSelected = false;
            return state with
            {
                ActiveApiResources = activeApiResources,
                ActiveCount = activeApiResources.Count,
                AvailableApiResources = availableApiResources,
                AvailableCount = availableApiResources.Count,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static ApiResourcesState ReduceUnassignApiResourcesSuccessAction(ApiResourcesState state, UnassignApiResourcesSuccessAction act)
        {
            var activeApiResources = state.ActiveApiResources.ToList();
            var availableApiResources = state.AvailableApiResources.ToList();
            var removedApiResources = activeApiResources.Where(r => !act.Resources.Contains(r.Value.Name)).ToList();
            availableApiResources.AddRange(removedApiResources);
            activeApiResources = activeApiResources.Where(r => act.Resources.Contains(r.Value.Name)).ToList();
            foreach (var resource in activeApiResources) resource.IsSelected = false;
            foreach (var resource in availableApiResources) resource.IsSelected = false;
            return state with
            {
                ActiveApiResources = activeApiResources,
                ActiveCount = activeApiResources.Count,
                AvailableApiResources = availableApiResources,
                AvailableCount = availableApiResources.Count,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static ApiResourcesState ReduceToggleActiveApiResourceSelectionAction(ApiResourcesState state, ToggleActiveApiResourceSelectionAction act)
        {
            var resources = state.ActiveApiResources?.ToList();
            if (resources == null) return state;
            var selectedResource = resources.Single(c => c.Value.Name == act.ResourceName);
            selectedResource.IsSelected = act.IsSelected;
            return state with
            {
                ActiveApiResources = resources
            };
        }

        [ReducerMethod]
        public static ApiResourcesState ReduceToggleActiveAllAvailableApiResourceSelectionAction(ApiResourcesState state, ToggleAllAvailableApiResourceSelectionAction act)
        {
            var resources = state.AvailableApiResources?.ToList();
            if (resources == null) return state;
            foreach (var resource in resources) resource.IsSelected = act.IsSelected;
            return state with
            {
                AvailableApiResources = resources
            };
        }

        [ReducerMethod]
        public static ApiResourcesState ReduceToggleActiveAllActiveApiResourceSelectionAction(ApiResourcesState state, ToggleAllActiveApiResourceSelectionAction act)
        {
            var resources = state.ActiveApiResources?.ToList();
            if (resources == null) return state;
            foreach (var resource in resources) resource.IsSelected = act.IsSelected;
            return state with
            {
                ActiveApiResources = resources
            };
        }

        [ReducerMethod]
        public static ApiResourcesState ReduceToggleAvailableApiResourceSelectionAction(ApiResourcesState state, ToggleAvailableApiResourceSelectionAction act)
        {
            var resources = state.AvailableApiResources?.ToList();
            if (resources == null) return state;
            var selectedResource = resources.Single(c => c.Value.Name == act.ResourceName);
            selectedResource.IsSelected = act.IsSelected;
            return state with
            {
                AvailableApiResources = resources
            };
        }

        [ReducerMethod]
        public static ApiResourcesState ReduceToggleAvailableAllApiResourceSelectionAction(ApiResourcesState state, ToggleAllAvailableApiResourceSelectionAction act)
        {
            var resources = state.AvailableApiResources?.ToList();
            if (resources == null) return state;
            foreach (var resource in resources) resource.IsSelected = act.IsSelected;
            return state with
            {
                AvailableApiResources = resources
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

        [ReducerMethod]
        public static AddApiResourceState ReduceStartAddApiResourceAction(AddApiResourceState state, StartAddApiResourceAction act) => new(isAdding: false, errorMessage: null);

        #endregion
    }
}
