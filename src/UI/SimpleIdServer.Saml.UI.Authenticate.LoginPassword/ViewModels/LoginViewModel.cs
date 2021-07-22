// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.Saml.DTOs;

namespace SimpleIdServer.Saml.UI.Authenticate.LoginPassword.ViewModels
{
    public class LoginViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
        public SAMLRequestDto Parameter { get; set; }

        public void Check(ModelStateDictionary modelStateDictionary)
        {
            if (Parameter == null)
            {
                modelStateDictionary.AddModelError("MissingParameter", "MissingParameter");
            }

            if (string.IsNullOrWhiteSpace(Parameter.SAMLRequest))
            {
                modelStateDictionary.AddModelError("MissingSamlRequest", "MissingSamlRequest");
            }
        }
    }
}