// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Fido.UI.ViewModels
{
    public class RegisterWebauthnViewModel
    {
        public string Login { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string SerializedAuthenticatorAttestationRawResponse { get; set; } = null!;
        public string BeginRegisterUrl { get; set; } = null!;
        public string EndRegisterUrl { get; set; } = null!;
    }
}
