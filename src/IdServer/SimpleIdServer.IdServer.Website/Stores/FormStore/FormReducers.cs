// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.FormStore;

public static class FormReducers
{
    #region FormState

    [ReducerMethod]
    public static FormState ReduceGetFormAction(FormState state, GetFormAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }
    
    [ReducerMethod]
    public static FormState ReduceUpdateFormAction(FormState state, UpdateFormAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static FormState ReducePublishFormAction(FormState state, PublishFormAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static FormState ReduceGetFormSuccessAction(FormState state, GetFormSuccessAction act)
    {
        return state with
        {
            IsLoading = false,
            Form = act.Form
        };
    }

    [ReducerMethod]
    public static FormState ReduceUpdateFormSuccessAction(FormState state, UpdateFormSuccessAction act)
    {
        return state with
        {
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static FormState ReducePublishFormSuccessAction(FormState state, PublishFormSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            Form = action.Form
        };
    }

    #endregion
}
