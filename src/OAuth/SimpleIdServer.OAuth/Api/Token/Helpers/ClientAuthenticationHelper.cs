// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.Helpers
{
    public interface IClientAuthenticationHelper
    {
        Task<OAuthClient> AuthenticateClient(JObject jObjHeader, JObject jObjBody, string issuerName, CancellationToken cancellationToken);
    }

    public class ClientAuthenticationHelper : IClientAuthenticationHelper
    {
        private readonly IAuthenticateClient _authenticateClient;

        public ClientAuthenticationHelper(IAuthenticateClient authenticateClient)
        {
            _authenticateClient = authenticateClient;
        }

        public async Task<OAuthClient> AuthenticateClient(JObject jObjHeader, JObject jObjBody, string issuerName, CancellationToken cancellationToken)
        {
            var authenticateInstruction = BuildAuthenticateInstruction(jObjHeader, jObjBody);
            var oauthClient = await _authenticateClient.Authenticate(authenticateInstruction, issuerName, cancellationToken);
            if (oauthClient == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT, ErrorMessages.BAD_CLIENT_CREDENTIAL);
            }

            return oauthClient;
        }

        private static AuthenticateInstruction BuildAuthenticateInstruction(JObject jObjHeader, JObject jObjBody)
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
                Certificate = null,
                RequestData = jObjBody
            };
        }
    }
}
