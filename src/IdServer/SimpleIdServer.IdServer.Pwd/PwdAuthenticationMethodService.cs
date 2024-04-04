// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Pwd
{
    public class PwdAuthenticationMethodService : IAuthenticationMethodService
    {
        public string Amr => Constants.Areas.Password;
        public string Name => "Password";
        public Type? OptionsType => typeof(IdServerPasswordOptions);
        public AuthenticationMethodCapabilities Capabilities => AuthenticationMethodCapabilities.USERAUTHENTICATION | AuthenticationMethodCapabilities.USERREGISTRATION;
        public bool IsCredentialExists(User user) => user.Credentials.Any(c => c.CredentialType == Constants.Areas.Password);
    }
}