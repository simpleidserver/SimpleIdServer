// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using System.Data;

namespace SimpleIdServer.IdServer.Website.Stores.ScopeStore
{
    public class ResourceReducers
    {
        #region SearchScopesState

        [ReducerMethod]
        public static SearchScopesState ReduceSearchScopesAction(SearchScopesState state, SearchScopesAction act) => new(isLoading: true, scopes: new List<Domains.Scope>());

        [ReducerMethod]
        public static SearchScopesState ReduceSearchScopesSuccessAction(SearchScopesState state, SearchScopesSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                Scopes = act.Scopes.Select(c => new SelectableScope(c)),
                Count = act.Count
            };
        }

        [ReducerMethod]
        public static SearchScopesState ReduceToggleScopeSelectionAction(SearchScopesState state, ToggleScopeSelectionAction act)
        {
            var scopes = state.Scopes?.ToList();
            if (scopes == null) return state;
            var selectedScope = scopes.Single(c => c.Value.Name == act.ScopeName);
            selectedScope.IsSelected = act.IsSelected;
            return state with
            {
                Scopes = scopes
            };
        }

        [ReducerMethod]
        public static SearchScopesState ReduceToggleAllScopeSelectionAction(SearchScopesState state, ToggleAllScopeSelectionAction act)
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
        public static SearchScopesState ReduceRemoveSelectedScopesAction(SearchScopesState state, RemoveSelectedScopesAction act)
        {
            if (act.IsRole) return state;
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchScopesState ReduceRemoveSelectedScopesSuccessAction(SearchScopesState state, RemoveSelectedScopesSuccessAction act)
        {
            if (act.IsRole) return state;
            var scopes = state.Scopes?.ToList();
            if (scopes == null) return state;
            scopes = scopes.Where(s => !act.ScopeIds.Contains(s.Value.Id)).ToList();
            return state with
            {
                Scopes = scopes,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static SearchScopesState ReduceAddScopeSuccessAction(SearchScopesState state, AddScopeSuccessAction act)
        {
            var scopes = state.Scopes?.ToList();
            scopes.Add(new SelectableScope(new Domains.Scope
            {
                Id = act.Id,
                Name = act.Name,
                Description = act.Description,
                Protocol = act.Protocol,
                Type = act.Type,
                IsExposedInConfigurationEdp = act.IsExposedInConfigurationEdp,
                CreateDateTime = DateTime.Now,
                UpdateDateTime = DateTime.Now
            })
            {
                IsNew = true
            });;
            return state with
            {
                Scopes = scopes
            };
        }

        [ReducerMethod]
        public static SearchScopesState ReduceUpdateScopeSuccessAction(SearchScopesState state, UpdateScopeSuccessAction act)
        {
            var scopes = state.Scopes;
            var resource = scopes?.SingleOrDefault(s => s.Value.Id == act.ScopeId);
            if(resource != null)
            {
                resource.Value.Description = act.Description;
                resource.Value.UpdateDateTime = DateTime.Now;
            }

            return state with
            {
                Scopes = scopes
            };
        }

        #endregion

        #region AddScopeState

        [ReducerMethod]
        public static AddScopeState ReduceAddIdentityScopeAction(AddScopeState state, AddIdentityScopeAction act) => state with
        {
            IsAdding = true
        };

        [ReducerMethod]
        public static AddScopeState ReduceAddScopeFailureAction(AddScopeState state, AddScopeFailureAction act) => state with
        {
            IsAdding = false,
            ErrorMessage = act.ErrorMessage
        };

        [ReducerMethod]
        public static AddScopeState ReduceAddScopeSuccessAction(AddScopeState state, AddScopeSuccessAction act) => state with
        {
            IsAdding = false
        };

        [ReducerMethod]
        public static AddScopeState ReduceStartAddScopeAction(AddScopeState state, StartAddScopeAction act) => state with
        {
            ErrorMessage = null
        };

        #endregion

        #region ScopeState

        [ReducerMethod]
        public static ScopeState ReduceGetScopeAction(ScopeState state, GetScopeAction act) => state with
        {
            IsLoading = true
        };

        [ReducerMethod]
        public static ScopeState ReduceGetScopeSuccessAction(ScopeState state, GetScopeSuccessAction act) => state with
        {
            IsLoading = false,
            Scope = act.Scope
        };

        [ReducerMethod]
        public static ScopeState ReduceGetScopeFailureAction(ScopeState state, GetScopeFailureAction act) => state with
        {
            IsLoading = false
        };

        [ReducerMethod]
        public static ScopeState ReduceUpdateScopeSuccessAction(ScopeState state, UpdateScopeSuccessAction act)
        {
            var resource = state.Scope;
            resource.UpdateDateTime = DateTime.Now;
            resource.Description = act.Description;
            resource.IsExposedInConfigurationEdp = act.IsExposedInConfigurationEdp;
            return state with
            {
                Scope = resource
            };
        }

        [ReducerMethod]
        public static ScopeState ReduceAddScopeClaimMapperSuccessAction(ScopeState state, AddScopeClaimMapperSuccessAction act)
        {
            var resource = state.Scope;
            var claimMappers = resource.ClaimMappers.ToList();
            claimMappers.Add(act.ClaimMapper);
            resource.ClaimMappers = claimMappers;
            return state with
            {
                Scope = resource
            };
        }

        [ReducerMethod]
        public static ScopeState ReduceUpdateScopeClaimMapperSuccessAction(ScopeState state, UpdateScopeClaimMapperSuccessAction act)
        {
            var resource = state.Scope;
            var claimMappers = resource.ClaimMappers.ToList();
            var claimMapper = claimMappers.Single(m => m.Name == act.ClaimMapper.Name);
            claimMappers.Remove(claimMapper);
            claimMappers.Add(act.ClaimMapper);
            resource.ClaimMappers = claimMappers;
            return state with
            {
                Scope = resource
            };
        }

        #endregion

        #region UpdateScopeState

        [ReducerMethod]
        public static UpdateScopeState ReduceUpdateScopeAction(UpdateScopeState state, UpdateScopeAction act) => state with
        {
            IsUpdating = true
        };

        [ReducerMethod]
        public static UpdateScopeState ReduceUpdateScopeSuccessAction(UpdateScopeState state, UpdateScopeSuccessAction act) => state with
        {
            IsUpdating = false
        };

        #endregion

        #region UpdateResourceMapperState

        [ReducerMethod]
        public static UpdateScopeMapperState ReduceAddScopeClaimMapperAction(UpdateScopeMapperState state, AddScopeClaimMapperAction act) => state with
        {
            IsUpdating = true
        };

        [ReducerMethod]
        public static UpdateScopeMapperState ReduceAddScopeClaimMapperSuccessAction(UpdateScopeMapperState state, AddScopeClaimMapperSuccessAction act) => state with
        {
            IsUpdating = false
        };

        [ReducerMethod]
        public static UpdateScopeMapperState ReduceAddScopeClaimMapperFailureAction(UpdateScopeMapperState state, AddScopeClaimMapperFailureAction act) => state with
        {
            IsUpdating = false,
            ErrorMessage = act.ErrorMessage
        };

        [ReducerMethod]
        public static UpdateScopeMapperState ReduceStartUpdateScopeMapperAction(UpdateScopeMapperState state, StartUpdateScopeMapperAction act) => state with
        {
            ErrorMessage = null
        };

        [ReducerMethod]
        public static UpdateScopeMapperState ReduceUpdateScopeClaimMapperAction(UpdateScopeMapperState state, UpdateScopeClaimMapperAction act) => state with
        {
            IsUpdating = true
        };

        [ReducerMethod]
        public static UpdateScopeMapperState ReduceUpdateScopeClaimMapperSuccessAction(UpdateScopeMapperState state, UpdateScopeClaimMapperSuccessAction act) => state with
        {
            IsUpdating = false
        };

        [ReducerMethod]
        public static UpdateScopeMapperState ReduceUpdateScopeClaimMapperFailureAction(UpdateScopeMapperState state, UpdateScopeClaimMapperFailureAction act) => state with
        {
            IsUpdating = false,
            ErrorMessage = act.ErrorMessage
        };

        #endregion

        #region ScopeMappersState

        [ReducerMethod]
        public static ScopeMappersState ReduceGetScopeAction(ScopeMappersState state, GetScopeAction act) => state with
        {
            IsLoading = true
        };

        [ReducerMethod]
        public static ScopeMappersState ReduceGetScopeSuccessAction(ScopeMappersState state, GetScopeSuccessAction act) => state with
        {
            IsLoading = false,
            Mappers = act.Scope.ClaimMappers.Select(m => new SelectableScopeMapper(m)),
            Count = act.Scope.ClaimMappers.Count()
        };

        [ReducerMethod]
        public static ScopeMappersState ReduceToggleScopeMapperSelectionAction(ScopeMappersState state, ToggleScopeMapperSelectionAction act)
        {
            var mappers = state.Mappers.ToList();
            var mapper = mappers.Single(m => m.Value.Id == act.ScopeMapperId);
            mapper.IsSelected = act.IsSelected;
            return state with
            {
                Mappers = mappers
            };
        }

        [ReducerMethod]
        public static ScopeMappersState ReduceToggleAllScopeMapperSelectionAction(ScopeMappersState state, ToggleAllScopeMapperSelectionAction act)
        {
            var mappers = state.Mappers.ToList();
            foreach (var mapper in mappers) mapper.IsSelected = act.IsSelected;
            return state with
            {
                Mappers = mappers
            };
        }

        [ReducerMethod]
        public static ScopeMappersState ReduceRemoveSelectedScopeMappersAction(ScopeMappersState state, RemoveSelectedScopeMappersAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static ScopeMappersState ReduceRemoveSelectedScopeMappersSuccessAction(ScopeMappersState state, RemoveSelectedScopeMappersSuccessAction act)
        {
            var mappers = state.Mappers.ToList();
            var filteredMappers = mappers.Where(m => !act.ScopeMapperIds.Contains(m.Value.Id)).ToList();
            return state with
            {
                Mappers = filteredMappers,
                IsLoading = false,
                Count = filteredMappers.Count
            };
        }

        [ReducerMethod]
        public static ScopeMappersState ReduceAddScopeClaimMapperSuccessAction(ScopeMappersState state, AddScopeClaimMapperSuccessAction act)
        {
            var mappers = state.Mappers.ToList();
            mappers.Add(new SelectableScopeMapper(act.ClaimMapper) { IsNew = true });
            return state with
            {
                Mappers = mappers,
                IsLoading = false,
                Count = mappers.Count
            };
        }

        #endregion

        #region RealmScopesState


        [ReducerMethod]
        public static RealmScopesState ReduceGetAllRealmScopesAction(RealmScopesState state, GetAllRealmScopesAction act) => state with
        {
            IsLoading = true
        };


        [ReducerMethod]
        public static RealmScopesState ReduceGetAllRealmScopesSuccessAction(RealmScopesState state, GetAllRealmScopesSuccessAction act) => state with
        {
            IsLoading = false,
            Scopes = act.Scopes
        };

        #endregion
    }
}