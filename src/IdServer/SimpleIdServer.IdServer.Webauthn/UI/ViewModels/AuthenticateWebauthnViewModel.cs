// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Webauthn.UI.ViewModels
{
    public class AuthenticateWebauthnViewModel : BaseAuthenticateViewModel
    {
        public AuthenticateWebauthnViewModel()
        {

        }

        public bool IsFidoCredentialsMissing { get; set; } = false;
        public string SerializedAuthenticatorAssertionRawResponse { get; set; }

        public override void CheckRequiredFields(User user, ModelStateDictionary modelStateDictionary)
        {
            if (string.IsNullOrWhiteSpace(ReturnUrl))
                modelStateDictionary.AddModelError("missing_return_url", "missing_return_url");

            if (string.IsNullOrWhiteSpace(Login))
                modelStateDictionary.AddModelError("missing_login", "missing_login");

            if (string.IsNullOrWhiteSpace(SerializedAuthenticatorAssertionRawResponse))
                modelStateDictionary.AddModelError("missing_assertion", "missing_assertion");
        }
    }
}
