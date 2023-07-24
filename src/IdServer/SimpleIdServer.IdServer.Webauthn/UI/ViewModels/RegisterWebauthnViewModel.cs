// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SimpleIdServer.IdServer.Webauthn.UI.ViewModels
{
    public class RegisterWebauthnViewModel
    {
        public string Login { get; set; }
        public string SerializedAuthenticatorAttestationRawResponse { get; set; }

        public void CheckRequiredFields(ModelStateDictionary modelStateDictionary)
        {
            if (string.IsNullOrWhiteSpace(Login))
                modelStateDictionary.AddModelError("missing_login", "missing_login");

            if (string.IsNullOrWhiteSpace(SerializedAuthenticatorAttestationRawResponse))
                modelStateDictionary.AddModelError("missing_attestation", "missing_attestation");
        }
    }
}
