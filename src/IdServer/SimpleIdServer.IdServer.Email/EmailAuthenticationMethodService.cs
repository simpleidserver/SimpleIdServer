// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Email
{
    public class EmailAuthenticationMethodService : IAuthenticationMethodService
    {
        public string Amr => Constants.AMR;
        public string Name => "Email";
        public Type? OptionsType => typeof(IdServerEmailOptions);
        public AuthenticationMethodCapabilities Capabilities => AuthenticationMethodCapabilities.USERAUTHENTICATION | AuthenticationMethodCapabilities.PUSHNOTIFICATION | AuthenticationMethodCapabilities.USERREGISTRATION;
        public bool IsCredentialExists(User user) => !string.IsNullOrEmpty(user.Email);
    }
}