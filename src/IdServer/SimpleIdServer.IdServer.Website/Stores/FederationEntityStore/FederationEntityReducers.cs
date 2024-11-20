// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Website.Pages;
using SimpleIdServer.OpenidFederation.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.FederationEntityStore;

public static class FederationEntityReducers
{
    #region SearchFederationEntitiesState

    [ReducerMethod]
    public static SearchFederationEntitiesState ReduceSearchFederationEntitiesAction(SearchFederationEntitiesState state, SearchFederationEntitiesAction act) => new(true, new List<FederationEntity>(), 0);

    [ReducerMethod]
    public static SearchFederationEntitiesState ReduceSearchFederationEntitiesAction(SearchFederationEntitiesState state, RemoveFederationEntitiesAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static SearchFederationEntitiesState ReduceSearchFederationEntitiesSuccessAction(SearchFederationEntitiesState state, SearchFederationEntitiesSuccessAction act)
    {
        return state with
        {
            IsLoading = false,
            FederationEntities = act.FederationEntities.Select(c => new SelectableFederationEntity(c)),
            Count = act.Count
        };
    }

    [ReducerMethod]
    public static SearchFederationEntitiesState ReduceAddTrustedAnchorAction(SearchFederationEntitiesState state, AddTrustedAnchorAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static SearchFederationEntitiesState ReduceAddTrustedAnchorSuccessAction(SearchFederationEntitiesState state, AddTrustedAnchorSuccessAction act)
    {
        var federationEntities = state.FederationEntities?.ToList();
        if (federationEntities == null) return state;
        federationEntities.Add(new SelectableFederationEntity(act.FederationEntity) { IsNew = true });
        return state with
        {
            FederationEntities = federationEntities,
            Count = federationEntities.Count(),
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static SearchFederationEntitiesState ReduceAddTrustedAnchorFailureAction(SearchFederationEntitiesState state, AddTrustedAnchorFailureAction act)
    {
        return state with
        {
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static SearchFederationEntitiesState ReduceRemoveFederationEntitiesSuccessAction(SearchFederationEntitiesState state, RemoveFederationEntitiesSuccessAction act)
    {
        var federationEntities = state.FederationEntities.ToList();
        var filteredFederationEntities = federationEntities.Where(f => !act.Ids.Contains(f.FederationEntity.Id));
        return state with
        {
            IsLoading = false,
            Count = filteredFederationEntities.Count(),
            FederationEntities = filteredFederationEntities
        };
    }

    [ReducerMethod]
    public static SearchFederationEntitiesState ReduceToggleFederationEntityAction(SearchFederationEntitiesState state, ToggleFederationEntityAction act)
    {
        var federationEntities = state.FederationEntities.ToList();
        var record = federationEntities.Single(e => e.FederationEntity.Id == act.Id);
        record.IsSelected = act.IsSelected;
        return state with
        {
            FederationEntities = federationEntities
        };
    }

    [ReducerMethod]
    public static SearchFederationEntitiesState ReduceToggleAllFederationEntitiesAction(SearchFederationEntitiesState state, ToggleAllFederationEntitiesAction act)
    {
        var federationEntities = state.FederationEntities;
        foreach (var federationEntity in federationEntities)
            federationEntity.IsSelected = act.IsSelected;
        return state with
        {
            FederationEntities = federationEntities
        };
    }

    #endregion

    #region AddFederationEntityState

    [ReducerMethod]
    public static AddFederationEntityState ReduceStartAddTrustAnchorAction(AddFederationEntityState state, StartAddTrustAnchorAction act)
    {
        return state with
        {
            IsAdding = false,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static AddFederationEntityState ReduceAddTrustedAnchorAction(AddFederationEntityState state, AddTrustedAnchorAction act)
    {
        return state with
        {
            IsAdding = true,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static AddFederationEntityState ReduceAddTrustedAnchorSuccessAction(AddFederationEntityState state, AddTrustedAnchorSuccessAction act)
    {
        return state with
        {
            IsAdding = false,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static AddFederationEntityState ReduceAddTrustedAnchorFailureAction(AddFederationEntityState state, AddTrustedAnchorFailureAction act)
    {
        return state with
        {
            IsAdding = false,
            ErrorMessage = act.ErrorMessage
        };
    }

    #endregion
}
