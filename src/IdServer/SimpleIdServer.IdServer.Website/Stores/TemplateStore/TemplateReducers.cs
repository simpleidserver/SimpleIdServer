// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.TemplateStore;

public class TemplateReducers
{

    [ReducerMethod]
    public static TemplateState ReduceGetActiveTemplateAction(TemplateState state, GetActiveTemplateAction act) => new(true);

    [ReducerMethod]
    public static TemplateState ReduceGetActiveTemplateSuccessAction(TemplateState state, GetActiveTemplateSuccessAction act)
    {
        return state with
        {
            IsLoading = false,
            Template = act.Template
        };
    }
}