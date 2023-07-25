// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SimpleIdServer.IdServer.Webauthn.UI.ViewModels
{
    public class RegisterWebauthnViewModel
    {
        public string Login { get; set; }
        public string DisplayName { get; set; }
        public string SerializedAuthenticatorAttestationRawResponse { get; set; }
    }
}
