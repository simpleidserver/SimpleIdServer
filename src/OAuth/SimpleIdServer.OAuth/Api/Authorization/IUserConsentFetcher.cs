// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;

namespace SimpleIdServer.OAuth.Api.Authorization
{
    public interface IUserConsentFetcher
    {
        OAuthConsent FetchFromAuthorizationRequest(OAuthUser oauthUser, JObject queryParameters);
        OAuthConsent BuildFromAuthorizationRequest(JObject queryParameters);
    }
}
