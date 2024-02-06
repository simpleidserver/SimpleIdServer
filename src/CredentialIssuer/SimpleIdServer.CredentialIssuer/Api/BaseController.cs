// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api;

public class BaseController : Controller
{
    protected IActionResult Build(ErrorResult error) => new ContentResult
    {
        StatusCode = (int)error.StatusCode,
        Content = JsonSerializer.Serialize(error),
        ContentType = "application/json"
    };

    protected string GetAccessToken()
    {
        var accessToken = Request.Headers
            .Single(h => h.Key == "Authorization")
            .Value
            .Single()
            .Split(" ")
            .Last();
        return accessToken;
    }
}

public class BaseValidationResult
{
    protected BaseValidationResult() { }

    protected BaseValidationResult(ErrorResult error)
    {
        ErrorResult = error;
    }

    public ErrorResult? ErrorResult { get; private set; }
}

public struct ErrorResult
{
    public ErrorResult()
    {

    }

    public ErrorResult(HttpStatusCode statusCode, string error, string? errorDescription = null)
    {
        StatusCode = statusCode;
        Error = error;
        ErrorDescription = errorDescription;
    }

    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }
    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Error { get; set; }
    [JsonPropertyName("error_description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorDescription { get; set; } = null;
}