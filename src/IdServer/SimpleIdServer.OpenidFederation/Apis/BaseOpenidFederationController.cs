// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OpenidFederation.Apis;

public class BaseOpenidFederationController : Controller
{
    protected IActionResult Error(HttpStatusCode statusCode, string errorCode, string errorMessage) => new ContentResult
    {
        StatusCode = (int)statusCode,
        Content = new JsonObject
        {
            ["error"] = errorCode,
            ["error_description"] = errorMessage
        }.ToJsonString(),
        ContentType = "application/json"
    };

    protected string GetAbsoluteUriWithVirtualPath(HttpRequest requestMessage)
    {
        var host = requestMessage.Host.Value;
        var http = "http://";
        if (requestMessage.IsHttps) http = "https://";
        var relativePath = requestMessage.PathBase.Value;
        return http + host + relativePath;
    }
}