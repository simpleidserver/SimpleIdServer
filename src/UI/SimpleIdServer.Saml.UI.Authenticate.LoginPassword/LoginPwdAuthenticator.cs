// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Idp;

namespace SimpleIdServer.Saml.UI.Authenticate.LoginPassword
{
    public class LoginPwdAuthenticator : IAuthenticator
    {
        public string AuthnContextClassRef => Constants.AuthnContextClassRef;
        public string Amr => Constants.AMR;
    }
}
