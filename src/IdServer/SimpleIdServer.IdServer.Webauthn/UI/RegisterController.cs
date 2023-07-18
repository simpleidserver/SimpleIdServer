// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fido2NetLib;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Webauthn.UI
{
    [Area(Constants.AMR)]
    public class RegisterController : Controller
    {
        private readonly IFido2 _fido2;

        public RegisterController(IFido2 fido2)
        {
            _fido2= fido2;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult MakeCredentialsOptions([FromBody] JsonObject jObj)
        {

            return null;
        }
    }
}
