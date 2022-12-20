// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Helpers;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Authenticate.Handlers
{
    public class OAuthClientSelfSignedTlsClientAuthenticationHandler : IOAuthClientAuthenticationHandler
    {
        private readonly IClientHelper _clientHelper;
        private readonly ILogger<OAuthClientSelfSignedTlsClientAuthenticationHandler> _logger;

        public OAuthClientSelfSignedTlsClientAuthenticationHandler(
            IClientHelper clientHelper,
            ILogger<OAuthClientSelfSignedTlsClientAuthenticationHandler> logger)
        {
            _clientHelper = clientHelper;
            _logger = logger;
        }

        /// <summary>
        /// Indicates that the client authentication to the authorization server will occur using mutual TLS with the client
        /// utilizing a self signed certificate.
        /// </summary>
        public const string AUTH_METHOD = "self_signed_tls_client_auth";
        public string AuthMethod => AUTH_METHOD;

        public async Task<bool> Handle(AuthenticateInstruction authenticateInstruction, Client client, string expectedIssuer, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            var certificate = authenticateInstruction.Certificate;
            if (certificate == null) throw new OAuthException(errorCode, ErrorMessages.NO_CLIENT_CERTIFICATE);
            await CheckCertificate(certificate, client, errorCode, cancellationToken);
            return true;
        }

        private async Task CheckCertificate(X509Certificate2 certificate, Client client, string errorCode, CancellationToken cancellationToken)
        {
            if (!certificate.IsSelfSigned())
            {
                _logger.LogError("the certificate is not self signed");
                throw new OAuthException(errorCode, ErrorMessages.CERTIFICATE_IS_NOT_SELF_SIGNED);
            }

            var jsonWebKeys = await _clientHelper.ResolveJsonWebKeys(client, cancellationToken);
            foreach(var jsonWebKey in jsonWebKeys)
            {
                var x5c = jsonWebKey.X5c;
                if (x5c == null || !x5c.Any()) continue;
                var x5cBase64Str = x5c.First();
                var clientCertificate = new X509Certificate2(Convert.FromBase64String(x5cBase64Str));
                if (clientCertificate.Thumbprint == certificate.Thumbprint) return;
            }

            throw new OAuthUnauthorizedException(ErrorCodes.INVALID_CLIENT, ErrorMessages.BAD_SELF_SIGNED_CERTIFICATE);
        }
    }
}
