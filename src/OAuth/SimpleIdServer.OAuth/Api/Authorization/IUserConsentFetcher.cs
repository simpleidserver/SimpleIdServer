// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OAuth.Api.Authorization
{
    public interface IUserConsentFetcher
    {
        Consent FetchFromAuthorizationRequest(User oauthUser, JsonObject queryParameters);
        Consent BuildFromAuthorizationRequest(JsonObject queryParameters);
    }
}
