// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.DTOs
{
    /// <summary>
    /// FIDO U2F metadata configuration.
    /// </summary>
    public static class U2FConfigurationResultNames
    {
        public const string RegistrationEndpoint = "registration_endpoint";
        public const string AuthenticationEndpoint = "authentication_endpoint";
        public const string Version = "version";
        public const string Issuer = "issuer";
    }
}
