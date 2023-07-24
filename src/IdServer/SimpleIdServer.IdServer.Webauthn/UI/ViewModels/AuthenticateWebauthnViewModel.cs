// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Webauthn.UI.ViewModels
{
    public class AuthenticateWebauthnViewModel : BaseAuthenticateViewModel
    {
        public AuthenticateWebauthnViewModel()
        {

        }

        public AuthenticateWebauthnViewModel(string returnUrl, string realm, string login, string clientName, string logoUri, string tosUri, string policyUri, bool isLoginMissing, bool isAuthInProgress, AmrAuthInfo amrAuthInfo)
        {
            ReturnUrl = returnUrl;
            Realm = realm;
            Login = login;
            ClientName = clientName;
            LogoUri = logoUri;
            TosUri = tosUri;
            PolicyUri = policyUri;
            IsFidoMissing = isLoginMissing;
            IsAuthInProgress = isAuthInProgress;
            AmrAuthInfo = amrAuthInfo;
        }

        public string Login { get; set; }
        public bool IsFidoMissing { get; set; } = false;
        public bool IsAuthInProgress { get; set; } = false;
        public string SerializedAuthenticatorAssertionRawResponse { get; set; }
    }
}
