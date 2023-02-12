// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using System.Data;

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
                Protocol = act.Protocol,
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

        #region ResourceState

        [ReducerMethod]
        public static ResourceState ReduceGetResourceAction(ResourceState state, GetResourceAction act) => state with
        {
            IsLoading = true
        };

        [ReducerMethod]
        public static ResourceState ReduceGetResourceSuccessAction(ResourceState state, GetResourceSuccessAction act) => state with
        {
            IsLoading = false,
            Resource = act.Resource
        };

        [ReducerMethod]
        public static ResourceState ReduceGetResourceFailureAction(ResourceState state, GetResourceFailureAction act) => state with
        {
            IsLoading = false
        };

        [ReducerMethod]
        public static ResourceState ReduceUpdateResourceSuccessAction(ResourceState state, UpdateResourceSuccessAction act)
        {
            var resource = state.Resource;
            resource.UpdateDateTime = DateTime.UtcNow;
            resource.Description = act.Description;
            resource.IsExposedInConfigurationEdp = act.IsExposedInConfigurationEdp;
            return state with
            {
                Resource = resource
            };
        }

        #endregion

        #region UpdateResourceState

        [ReducerMethod]
        public static UpdateResourceState ReduceUpdateResourceAction(UpdateResourceState state, UpdateResourceAction act) => state with
        {
            IsUpdating = true
        };

        [ReducerMethod]
        public static UpdateResourceState ReduceUpdateResourceSuccessAction(UpdateResourceState state, UpdateResourceSuccessAction act) => state with
        {
            IsUpdating = false
        };

        #endregion

        #region UpdateResourceMapperState

        [ReducerMethod]
        public static UpdateResourceMapperState ReduceAddResourceClaimMapperAction(UpdateResourceMapperState state, AddResourceClaimMapperAction act) => state with
        {
            IsUpdating = true
        };

        [ReducerMethod]
        public static UpdateResourceMapperState ReduceAddResourceClaimMapperSuccessAction(UpdateResourceMapperState state, AddResourceClaimMapperSuccessAction act) => state with
        {
            IsUpdating = false
        };

        [ReducerMethod]
        public static UpdateResourceMapperState ReduceAddResourceClaimMapperFailureAction(UpdateResourceMapperState state, AddResourceClaimMapperFailureAction act) => state with
        {
            IsUpdating = false,
            ErrorMessage = act.ErrorMessage
        };

        #endregion

        #region ResourceMappersState

        [ReducerMethod]
        public static ResourceMappersState ReduceGetResourceAction(ResourceMappersState state, GetResourceAction act) => state with
        {
            IsLoading = true
        };

        [ReducerMethod]
        public static ResourceMappersState ReduceGetResourceSuccessAction(ResourceMappersState state, GetResourceSuccessAction act) => state with
        {
            IsLoading = false,
            Mappers = act.Resource.ClaimMappers.Select(m => new SelectableResourceMapper(m)),
            Count = act.Resource.ClaimMappers.Count()
        };

        [ReducerMethod]
        public static ResourceMappersState ReduceToggleResourceMapperSelectionAction(ResourceMappersState state, ToggleResourceMapperSelectionAction act)
        {
            var mappers = state.Mappers.ToList();
            var mapper = mappers.Single(m => m.Value.Id == act.ResourceMapperId);
            mapper.IsSelected = act.IsSelected;
            return state with
            {
                Mappers = mappers
            };
        }

        [ReducerMethod]
        public static ResourceMappersState ReduceToggleAllResourceMapperSelectionAction(ResourceMappersState state, ToggleAllResourceMapperSelectionAction act)
        {
            var mappers = state.Mappers.ToList();
            foreach (var mapper in mappers) mapper.IsSelected = act.IsSelected;
            return state with
            {
                Mappers = mappers
            };
        }

        [ReducerMethod]
        public static ResourceMappersState ReduceRemoveSelectedResourceMappersAction(ResourceMappersState state, RemoveSelectedResourceMappersAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static ResourceMappersState ReduceRemoveSelectedResourceMappersSuccessAction(ResourceMappersState state, RemoveSelectedResourceMappersSuccessAction act)
        {
            var mappers = state.Mappers.ToList();
            var filteredMappers = mappers.Where(m => !act.ResourceMapperIds.Contains(m.Value.Id)).ToList();
            return state with
            {
                Mappers = filteredMappers,
                IsLoading = false,
                Count = filteredMappers.Count
            };
        }

        [ReducerMethod]
        public static ResourceMappersState ReduceAddResourceClaimMapperSuccessAction(ResourceMappersState state, AddResourceClaimMapperSuccessAction act)
        {
            var mappers = state.Mappers.ToList();
            mappers.Add(new SelectableResourceMapper(act.ClaimMapper) { IsNew = true });
            return state with
            {
                Mappers = mappers,
                IsLoading = false,
                Count = mappers.Count
            };
        }

        #endregion
    }
}
