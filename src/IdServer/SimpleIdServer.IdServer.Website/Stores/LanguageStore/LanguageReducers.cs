// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.LanguageStore;

public static class LanguageReducers
{
    #region LanguagesState

    [ReducerMethod]
    public static LanguagesState ReduceGetLanguagesAction(LanguagesState state, GetLanguagesAction act) => new LanguagesState();

    [ReducerMethod]
    public static LanguagesState ReduceGetLanguagesSuccessAction(LanguagesState state, GetLanguagesSuccessAction act)
        => new LanguagesState(act.Languages, false);

    #endregion
}