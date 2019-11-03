// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ProtectAPIFromUndesirableUsers.Api.Controllers
{
    [Route("bankaccounts")]
    public class BankAccountController : Controller
    {
        [HttpGet(".me")]
        [Authorize("Authenticated")]
        public IActionResult GetMyProfile()
        {
            return new OkObjectResult(new JObject
            {
                { "amount", "2000" }
            });
        }
    }
}
