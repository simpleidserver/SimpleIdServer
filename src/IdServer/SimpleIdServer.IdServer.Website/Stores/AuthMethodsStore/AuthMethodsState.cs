// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Api.AuthenticationMethods;

namespace SimpleIdServer.IdServer.Website.Stores.AuthMethodsStore;

[FeatureState]
public record AuthMethodsState
{
	public AuthMethodsState()
	{

	}

	public AuthMethodsState(IEnumerable<AuthenticationMethodResult> authenticationMethods, bool isLoading)
	{
		AuthenticationMethods = authenticationMethods;
		IsLoading = isLoading;
		Count = authenticationMethods == null ? 0 : authenticationMethods.Count();
    }

	public IEnumerable<AuthenticationMethodResult> AuthenticationMethods { get; set; }
	public bool IsLoading { get; set; }
	public int Count { get; set; }
}
