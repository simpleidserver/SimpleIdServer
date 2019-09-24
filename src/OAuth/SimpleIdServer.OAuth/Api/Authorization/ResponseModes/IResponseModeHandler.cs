// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace SimpleIdServer.OAuth.Api.Authorization.ResponseModes
{
    public interface IResponseModeHandler
    {
        void Handle(JObject queryParams, RedirectURLAuthorizationResponse authorizationResponse, HttpContext httpContext);
    }
}