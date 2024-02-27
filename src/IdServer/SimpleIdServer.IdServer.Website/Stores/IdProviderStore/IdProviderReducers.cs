// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Api.AuthenticationSchemeProviders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.IdProviderStore
{
    public static class IdProviderReducers
    {
        #region SearchIdProvidersState

        [ReducerMethod]
        public static SearchIdProvidersState ReduceGetIdProvidersAction(SearchIdProvidersState state, SearchIdProvidersAction act) => new(idProviders: new List<AuthenticationSchemeProviderResult>(), true);

        [ReducerMethod]
        public static SearchIdProvidersState ReduceSearchIdProvidersSuccessAction(SearchIdProvidersState state, SearchIdProvidersSuccessAction act) => new(idProviders: act.IdProviders, false)
        {
            Count = act.Count
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
            idProviders.Add(new SelectableIdProvider(new AuthenticationSchemeProviderResult { CreateDateTime = DateTime.Now, UpdateDateTime = DateTime.Now, Description = act.Description, DisplayName = act.DisplayName, Name = act.Name })
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
            idProvider.Value.UpdateDateTime = DateTime.Now;
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
            provider.UpdateDateTime = DateTime.Now;
            return state with
            {
                Provider = provider
            };
        }

        [ReducerMethod]
        public static IdProviderState ReduceUpdateAuthenticationSchemeProviderPropertiesAction(IdProviderState state, UpdateAuthenticationSchemeProviderPropertiesAction act)
        {
            var provider = state.Provider;
            provider.UpdateDateTime = DateTime.Now;
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

        [ReducerMethod]
        public static UpdateIdProviderState ReduceAddAuthenticationSchemeProviderMapperAction(UpdateIdProviderState state, AddAuthenticationSchemeProviderMapperAction act)
        {
            return state with
            {
                IsUpdating = true,
                ErrorMessage = null
            };
        }

        [ReducerMethod]
        public static UpdateIdProviderState ReduceAddAuthenticationSchemeProviderMapperSuccessAction(UpdateIdProviderState state, AddAuthenticationSchemeProviderMapperSuccessAction act)
        {
            return state with
            {
                IsUpdating = false,
                ErrorMessage = null
            };
        }

        [ReducerMethod]
        public static UpdateIdProviderState ReduceUpdateAuthenticationSchemeProviderMapperAction(UpdateIdProviderState state, UpdateAuthenticationSchemeProviderMapperAction act)
        {
            return state with
            {
                IsUpdating = true,
                ErrorMessage = null
            };
        }

        [ReducerMethod]
        public static UpdateIdProviderState ReduceUpdateAuthenticationSchemeProviderMapperSuccessAction(UpdateIdProviderState state, UpdateAuthenticationSchemeProviderMapperSuccessAction act)
        {
            return state with
            {
                IsUpdating = false,
                ErrorMessage = null
            };
        }

        #endregion

        #region IdProviderMappersState

        [ReducerMethod]
        public static IdProviderMappersState ReduceGetIdProviderAction(IdProviderMappersState state, GetIdProviderAction act) => new IdProviderMappersState(new List<AuthenticationSchemeProviderMapperResult>(), true) { Count = 0 };

        [ReducerMethod]
        public static IdProviderMappersState ReduceGetIdProviderSuccessAction(IdProviderMappersState state, GetIdProviderSuccessAction act) => new IdProviderMappersState(act.IdProvider.Mappers, false)
        {
            Count = act.IdProvider.Mappers.Count
        };

        [ReducerMethod]
        public static IdProviderMappersState ReduceAddAuthenticationSchemeProviderMapperSuccessAction(IdProviderMappersState state, AddAuthenticationSchemeProviderMapperSuccessAction act)
        {
            var mappers = state.Mappers.ToList();
            mappers.Add(new SelectableAuthenticationSchemeProviderMapper(new AuthenticationSchemeProviderMapperResult
            {
                Id = act.Id,
                MapperType = act.MapperType,
                Name = act.Name,
                SourceClaimName = act.SourceClaimName,
                TargetUserAttribute = act.TargetUserAttribute,
                TargetUserProperty = act.TargetUserProperty
            })
            {
                IsNew = true
            });
            return state with
            {
                Mappers = mappers,
                Count = mappers.Count
            };
        }

        [ReducerMethod]
        public static IdProviderMappersState ReduceToggleAuthenticationSchemeProviderMapperAction(IdProviderMappersState state, ToggleAuthenticationSchemeProviderMapperAction act)
        {
            var mappers = state.Mappers.ToList();
            var mapper = mappers.First(m => m.Value.Id == act.MapperId);
            mapper.IsSelected = act.IsSelected;
            return state with
            {
                Mappers = mappers
            };
        }

        [ReducerMethod]
        public static IdProviderMappersState ReduceToggleAllAuthenticationSchemeProviderSelectionAction(IdProviderMappersState state, ToggleAllAuthenticationSchemeProviderSelectionAction act)
        {
            var mappers = state.Mappers.ToList();
            foreach (var mapper in mappers)
                mapper.IsSelected = act.IsSelected;
            return state with
            {
                Mappers = mappers
            };
        }

        [ReducerMethod]
        public static IdProviderMappersState ReduceReduceRemoveSelectedAuthenticationSchemeProviderMappersAction(IdProviderMappersState state, RemoveSelectedAuthenticationSchemeProviderMappersAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static IdProviderMappersState ReduceRemoveSelectedAuthenticationSchemeProviderMappersSuccessAction(IdProviderMappersState state, RemoveSelectedAuthenticationSchemeProviderMappersSuccessAction act)
        {
            var mappers = state.Mappers.ToList();
            mappers = mappers.Where(m => !act.MapperIds.Contains(m.Value.Id)).ToList();
            return state with
            {
                Mappers = mappers,
                Count = mappers.Count,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static IdProviderMappersState ReduceUpdateAuthenticationSchemeProviderMapperSuccessAction(IdProviderMappersState state, UpdateAuthenticationSchemeProviderMapperSuccessAction action)
        {
            var mappers = state.Mappers.ToList();
            var mapper = mappers.First(m => m.Value.Id == action.Id);
            mapper.Value.Name = action.Name;
            mapper.Value.SourceClaimName = action.SourceClaimName;
            mapper.Value.TargetUserAttribute = action.TargetUserAttribute;
            mapper.Value.TargetUserProperty = action.TargetUserProperty;
            return state with
            {
                Mappers = mappers,
                IsLoading = false
            };
        }

        #endregion
    }
}
