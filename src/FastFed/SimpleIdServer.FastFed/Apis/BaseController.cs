// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace SimpleIdServer.FastFed.Apis;

public class BaseController : Controller
{
    protected IActionResult BuildResult<T>(ValidationResult<T> result, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        if (result.HasError)
            return BuildError(result.ErrorCode, result.ErrorDescriptions, statusCode);
        return Ok(result.Result);
    }

    protected IActionResult BuildError(string errorCode, string errorDescription, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => BuildError(errorCode, new List<string> { errorDescription }, statusCode);

    protected IActionResult BuildError(string errorCode, List<string> errorDescriptions, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ContentResult
        {
            StatusCode = (int)statusCode,
            Content = JsonSerializer.Serialize(new ErrorResult
            {
                ErrorCode = errorCode,
                ErrorDescriptions = errorDescriptions
            }),
            ContentType = "application/json"
        };
    }
}
