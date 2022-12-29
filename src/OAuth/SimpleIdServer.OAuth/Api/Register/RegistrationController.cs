// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register
{
    public class RegistrationController : Controller
    {
        public RegistrationController()
        {

        }


        [HttpPost]
        public async Task<IActionResult> Add()
        {
            return null;
        }
    }
}
