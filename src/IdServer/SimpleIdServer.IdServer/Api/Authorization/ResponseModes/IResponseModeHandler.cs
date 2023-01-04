// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api.Authorization.ResponseModes
{
    public interface IResponseModeHandler
    {
        void Handle(JsonObject queryParams, RedirectURLAuthorizationResponse authorizationResponse, HttpContext httpContext);
    }
}