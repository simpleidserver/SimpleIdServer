// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Otp
{
    public class OtpAuthenticationMethodService : IAuthenticationMethodService
    {
        public string Amr => Constants.Amr;
        public string Name => "OTP";
        public Type? OptionsType => null;
        public AuthenticationMethodCapabilities Capabilities => AuthenticationMethodCapabilities.USERAUTHENTICATION;
        public bool IsRegistrationSupported => false;
        public bool IsCredentialExists(User user) => user.ActiveOTP != null;
    }
}