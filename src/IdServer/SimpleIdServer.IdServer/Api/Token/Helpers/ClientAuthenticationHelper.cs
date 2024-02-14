// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Authenticate;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Resources;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Helpers
{
    public interface IClientAuthenticationHelper
    {
        Task<Client> AuthenticateClient(string realm, JsonObject jObjHeader, JsonObject jObjBody, X509Certificate2 certificate, string issuerName, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT);
        bool TryGetClientId(string realm, JsonObject jObjHeader, JsonObject jObjBody, X509Certificate2 certificate, out string clientId);
    }

    public class ClientAuthenticationHelper : IClientAuthenticationHelper
    {
        private readonly IAuthenticateClient _authenticateClient;

        public ClientAuthenticationHelper(IAuthenticateClient authenticateClient)
        {
            _authenticateClient = authenticateClient;
        }

        public async Task<Client> AuthenticateClient(string realm, JsonObject jObjHeader, JsonObject jObjBody, X509Certificate2 certificate, string issuerName, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            var authenticateInstruction = BuildAuthenticateInstruction(realm, jObjHeader, jObjBody, certificate);
            var oauthClient = await _authenticateClient.Authenticate(authenticateInstruction, issuerName, cancellationToken, errorCode: errorCode);
            if (oauthClient == null) throw new OAuthException(errorCode, Global.BadClientCredential);
            return oauthClient;
        }

        public bool TryGetClientId(string realm, JsonObject jObjHeader, JsonObject jObjBody, X509Certificate2 certificate, out string clientId)
        {
            var authenticateInstruction = BuildAuthenticateInstruction(realm, jObjHeader, jObjBody, certificate);
            return _authenticateClient.TryGetClientId(authenticateInstruction, out clientId);
        }

        private AuthenticateInstruction BuildAuthenticateInstruction(string realm, JsonObject jObjHeader, JsonObject jObjBody, X509Certificate2 certificate)
        {
            var clientCredential = jObjHeader.GetClientCredentials();
            return new AuthenticateInstruction
            {
                ClientAssertion = jObjBody.GetClientAssertion(),
                ClientAssertionType = jObjBody.GetClientAssertionType(),
                ClientIdFromHttpRequestBody = jObjBody.GetClientId(),
                ClientSecretFromHttpRequestBody = jObjBody.GetClientSecret(),
                ClientIdFromAuthorizationHeader = clientCredential == null ? null : clientCredential.ClientId,
                ClientSecretFromAuthorizationHeader = clientCredential == null ? null : clientCredential.ClientSecret,
                Certificate = certificate,
                RequestData = jObjBody,
                Realm = realm
            };
        }
    }
}
