// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Sms
{
    public class SmsAuthenticationMethodService : IAuthenticationMethodService
    {
        public string Amr => Constants.AMR;
        public string Name => "Sms";
        public Type? OptionsType => typeof(IdServerSmsOptions);
        public AuthenticationMethodCapabilities Capabilities => AuthenticationMethodCapabilities.USERAUTHENTICATION | AuthenticationMethodCapabilities.PUSHNOTIFICATION | AuthenticationMethodCapabilities.USERREGISTRATION;
        public bool IsCredentialExists(User user) => user.OAuthUserClaims.Any(c => c.Name == JwtRegisteredClaimNames.PhoneNumber);
    }
}