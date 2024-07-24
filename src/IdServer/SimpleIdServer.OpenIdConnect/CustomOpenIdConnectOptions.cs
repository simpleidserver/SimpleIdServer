// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.OpenIdConnect
{
    public class CustomOpenIdConnectOptions : OpenIdConnectOptions
    {
        public ClientAuthenticationTypes ClientAuthenticationType { get; private set; } = ClientAuthenticationTypes.CLIENT_SECRET_POST;
        public RequestTypes RequestType { get; private set; }
        /// <summary>
        /// Certificate used during the tls_client_auth authentication.
        /// </summary>
        public X509Certificate2 MTLSCertificate { get; private set; } = null;
        /// <summary>
        /// Credentials used to sign the 'request' and / or the 'client_assertion'.
        /// </summary>
        public SigningCredentials SigningCredentials { get; private set; } = null;
        public bool IsDPoPUsed { get; private set; } = false;
        public bool IsDPoPNonceEnabled { get; private set; } = false;
        public double DPoPSecurityKeyRotationInSeconds { get; private set; } = 5 * 60;

        /// <summary>
        /// Use MTLS as Proof of Possession.
        /// </summary>
        /// <param name="mtlsCertificate"></param>
        /// <param name="signingCredentials"></param>
        public void UseMTLSProof(X509Certificate2 mtlsCertificate, SigningCredentials signingCredentials)
        {
            RequestType = RequestTypes.REQUEST;
            ClientAuthenticationType = ClientAuthenticationTypes.TLS_CLIENT_AUTH;
            MTLSCertificate = mtlsCertificate;
            SigningCredentials = signingCredentials;
            IsDPoPUsed = false;
        }

        /// <summary>
        /// Use DPoP as Proof of Possession.
        /// </summary>
        /// <param name="signingCredentials"></param>
        public void UseDPoPProof(SigningCredentials signingCredentials, bool isNonceEnabled = false, double dpopSecuritykeyRotationInSeconds = 300)
        {
            RequestType = RequestTypes.REQUEST;
            ClientAuthenticationType = ClientAuthenticationTypes.PRIVATE_KEY_JWT;
            SigningCredentials = signingCredentials;
            IsDPoPUsed = true;
            IsDPoPNonceEnabled = isNonceEnabled;
            DPoPSecurityKeyRotationInSeconds = dpopSecuritykeyRotationInSeconds;
        }

        public void UseFederationAutomaticRegistration(SigningCredentials signingCredentials)
        {
            RequestType = RequestTypes.REQUEST;
            ClientAuthenticationType = ClientAuthenticationTypes.PRIVATE_KEY_JWT;
            SigningCredentials = signingCredentials;
        }

        /// <summary>
        /// Send large authorization by using PAR.
        /// </summary>
        public void EnableLargeRequest()
        {
            RequestType = RequestTypes.PAR;
        }
    }

    public enum ClientAuthenticationTypes
    {
        CLIENT_SECRET_POST = 0,
        TLS_CLIENT_AUTH = 1,
        PRIVATE_KEY_JWT = 2
    }

    public enum RequestTypes
    {
        NONE = 0,
        REQUEST = 1,
        PAR = 2
    }
}