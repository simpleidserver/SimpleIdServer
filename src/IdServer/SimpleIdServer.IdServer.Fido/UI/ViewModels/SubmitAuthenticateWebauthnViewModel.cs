// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Fido.UI.ViewModels
{
    public class SubmitAuthenticateWebauthnViewModel
    {
        public string Login { get; set; }
        public string SerializedAuthenticatorAssertionRawResponse { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberLogin { get; set; }
    }
}
