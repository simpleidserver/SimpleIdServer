// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Api.AuthenticationMethods;

namespace SimpleIdServer.IdServer.Website.Stores.AuthMethodsStore;

public class AuthMethodReducers
{
    #region AuthMethodsState

    [ReducerMethod]
    public static AuthMethodsState Reduce(AuthMethodsState state, GetAllAuthMethodAction act) => new(isLoading: true, authenticationMethods: new List<AuthenticationMethodResult>());

    [ReducerMethod]
    public static AuthMethodsState Reduce(AuthMethodsState state, GetAllAuthMethodSuccessAction act) => new(isLoading: false, authenticationMethods: act.AuthMethods);

    #endregion

    #region AuthMethodState

    [ReducerMethod]
    public static AuthMethodState Reduce(AuthMethodState state, GetAuthMethodAction act) => new(null, true);

    [ReducerMethod]
    public static AuthMethodState Reduce(AuthMethodState state, GetAuthMethodSuccessAction act) => new(act.AuthMethod, false);

    [ReducerMethod]
    public static AuthMethodState Reduce(AuthMethodState state, GetAuthMethodFailureAction act)
    {
        return state with
        {
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static AuthMethodState Reduce(AuthMethodState state, UpdateAuthMethodAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static AuthMethodState Reduce(AuthMethodState state, UpdateAuthMethodSuccessAction act)
    {
        var authenticationMethod = state.AuthenticationMethod;
        authenticationMethod.Values = act.Values;
        return state with
        {
            IsLoading = false,
            AuthenticationMethod = authenticationMethod
        };
    }

    [ReducerMethod]
    public static AuthMethodState Reduce(AuthMethodState state, UpdateAuthMethodFailureAction act)
    {
        return state with
        {
            IsLoading = false
        };
    }

    #endregion
}
