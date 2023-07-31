// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;

namespace SimpleIdServer.IdServer.Fido.UI.Webauthn
{
    [Area(Constants.AMR)]
    public class RegisterController : BaseController
    {
        public RegisterController() { }

        [HttpGet]
        public IActionResult Index([FromRoute] string prefix)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            if (!string.IsNullOrWhiteSpace(prefix))
                prefix = $"{prefix}/";
            return View(new RegisterWebauthnViewModel
            {
                Login = string.Empty,
                BeginRegisterUrl = $"{issuer}/{prefix}{Constants.EndPoints.BeginRegister}",
                EndRegisterUrl = $"{issuer}/{prefix}{Constants.EndPoints.EndRegister}"
            });
        }
    }
}
