// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim;

public class SCIMHostEvents
{
    public Func<HttpContext, Exception, CancellationToken, Task<IActionResult>> OnInternalServerError = (context, e, c) => Task.FromResult((IActionResult)null);
}
