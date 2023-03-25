// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.OpenIdConnect
{
    public class CustomOpenIdConnectOptions : OpenIdConnectOptions
    {
        public ClientAuthenticationTypes ClientAuthenticationType { get; set; } = ClientAuthenticationTypes.CLIENT_SECRET_POST;
        public RequestTypes RequestType { get; set; } = RequestTypes.NONE;
        /// <summary>
        /// Certificate used during the tls_client_auth authentication.
        /// </summary>
        public X509Certificate2 MTLSCertificate { get; set; } = null;
        /// <summary>
        /// Credentials used to sign the 'request' parameter.
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; } = null;
        /// <summary>
        /// Use Pushed Authorization Request (PAR) https://www.rfc-editor.org/rfc/rfc9126
        /// </summary>
        public bool UsePushedAuthorizationRequest { get; set; }
    }

    public enum ClientAuthenticationTypes
    {
        CLIENT_SECRET_POST = 0,
        TLS_CLIENT_AUTH = 1
    }

    public enum RequestTypes
    {
        NONE = 0,
        REQUEST = 1,
        REQUEST_URI = 2,
        PAR = 3
    }
}