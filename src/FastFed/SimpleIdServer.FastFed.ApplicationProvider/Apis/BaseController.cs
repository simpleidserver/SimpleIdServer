// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace SimpleIdServer.FastFed.ApplicationProvider.Apis;

public class BaseController : Controller
{
    protected IActionResult BuildResult<T>(ValidationResult<T> result)
    {
        if (result.HasError) return new ContentResult
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Content = JsonSerializer.Serialize(new ErrorResult
            {
                ErrorCode = result.ErrorCode,
                ErrorDescription = result.ErrorDescription
            }),
            ContentType = "application/json"
        };
        return Ok(result.Result);
    }
}
