// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Website.Stores.AuthMethodsStore;

namespace SimpleIdServer.IdServer.Website.Stores.AcrsStore;

public static class AcrReducers
{
    #region AcrsState

    [ReducerMethod]
    public static AcrsState ReduceGetAllAcrAction(AcrsState state, GetAllAcrsAction action) => new(true, new List<Domains.AuthenticationContextClassReference>());

    [ReducerMethod]
    public static AcrsState ReduceGetAllAcrsSuccessAction(AcrsState state, GetAllAcrsSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            Count = action.Acrs.Count(),
            Acrs = action.Acrs.Select(a => new SelectableAcr(a)).ToList()
        };
    }

    [ReducerMethod]
    public static AcrsState ReduceAddAcrAction(AcrsState state, AddAcrAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static AcrsState ReduceAddAcrSuccessAction(AcrsState state, AddAcrSuccessAction action)
    {
        var result = state.Acrs.ToList();
        result.Add(new SelectableAcr(action.Acr)
        {
            IsNew = true
        });
        return state with
        {
            Count = result.Count,
            Acrs = result,
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static AcrsState ReduceAddAcrFailureAction(AcrsState state, AddAcrFailureAction action)
    {
        return state with
        {
            IsLoading = false
        };
    }

    [ReducerMethod] 
    public static AcrsState ReduceDeleteSelectedAcrsAction(AcrsState state, DeleteSelectedAcrsAction action)
    {
        return state with
        {
            IsLoading = true 
        };
    }

    [ReducerMethod]
    public static AcrsState ReduceDeleteSelectedAcrsSuccessAction(AcrsState state, DeleteSelectedAcrsSuccessAction action)
    {
        var acrs = state.Acrs.ToList();
        acrs = acrs.Where(a => !action.Ids.Contains(a.Value.Id)).ToList();
        return state with
        {
            Count = acrs.Count,
            Acrs = acrs,
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static AcrsState ReduceToggleAllAcrSelectionAction(AcrsState state, ToggleAllAcrSelectionAction action)
    {
        var acrs = state.Acrs.ToList();
        foreach (var acr in acrs) acr.IsSelected = action.IsSelected;
        return state with
        {
            Acrs = acrs
        };
    }

    [ReducerMethod]
    public static AcrsState ReduceToggleAcrSelectionAction(AcrsState state, ToggleAcrSelectionAction action)
    {
        var acrs = state.Acrs.ToList();
        var acr = acrs.Single(a => a.Value.Name == action.Name);
        acr.IsSelected = action.IsSelected;
        return state with
        {
            Acrs = acrs
        };
    }

    #endregion

    #region UpdateAcrState

    [ReducerMethod]
    public static UpdateAcrState ReduceAddAcrAction(UpdateAcrState state, AddAcrAction action) => new(null)
    {
        IsUpdating = true
    };

    [ReducerMethod]
    public static UpdateAcrState ReduceAddAcrSuccessAction(UpdateAcrState state, AddAcrSuccessAction action)
    {
        return state with
        {
            ErrorMessage = null,
            IsUpdating = false
        };
    }

    [ReducerMethod]
    public static UpdateAcrState ReduceAddAcrFailureAction(UpdateAcrState state, AddAcrFailureAction action)
    {
        return state with
        {
            ErrorMessage = action.ErrorMessage,
            IsUpdating = false
        };
    }

    [ReducerMethod]
    public static UpdateAcrState ReduceDeleteSelectedAcrsAction(UpdateAcrState state, DeleteSelectedAcrsAction action) => new(null)
    {
        IsUpdating = true
    };

    [ReducerMethod]
    public static UpdateAcrState ReduceDeleteSelectedAcrsSuccessAction(UpdateAcrState state, DeleteSelectedAcrsSuccessAction action) => new(null)
    {
        IsUpdating = false
    };

    [ReducerMethod]
    public static UpdateAcrState ReduceStartAddAcrMethodAction(UpdateAcrState state, StartAddAcrMethodAction action) => new(null)
    {
        IsUpdating = false
    };

    #endregion

    #region AuthenticationFormsState

    [ReducerMethod]
    public static AuthenticationFormsState ReduceGetAllAuthenticationFormsAction(AuthenticationFormsState state, GetAllAuthenticationFormsAction action) => new(true);

    [ReducerMethod]
    public static AuthenticationFormsState ReduceGetAllAuthenticationFormsSuccessAction(AuthenticationFormsState state, GetAllAuthenticationFormsSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            FormRecords = action.AuthenticationForms
        };
    }

    #endregion
}
