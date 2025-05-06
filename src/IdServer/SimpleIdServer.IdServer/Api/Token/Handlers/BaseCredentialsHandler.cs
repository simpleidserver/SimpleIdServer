// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public abstract class BaseCredentialsHandler : IGrantTypeHandler
    {
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IdServerHostOptions _options;

        public BaseCredentialsHandler(IClientAuthenticationHelper clientAuthenticationHelper, IEnumerable<ITokenProfile> tokenProfiles, IOptions<IdServerHostOptions> options)
        {
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _tokenProfiles = tokenProfiles;
            _options = options.Value;
        }

        public abstract string GrantType { get; }

        public abstract Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken);

        protected IdServerHostOptions Options => _options;

        protected async Task<Client> AuthenticateClient(HandlerContext context, CancellationToken cancellationToken)
        {
            var oauthClient = await _clientAuthenticationHelper.AuthenticateClient(context.Realm, context.Request.HttpHeader, context.Request.RequestData, context.Request.Certificate, context.GetIssuer(), cancellationToken);
            CheckGrantType(oauthClient);
            return oauthClient;
        }

        protected async Task Authenticate(HandlerContext context, Client client, CancellationToken cancellationToken)
        {
            await _clientAuthenticationHelper.AuthenticateClient(client, context.Realm, context.Request.HttpHeader, context.Request.RequestData, context.Request.Certificate, context.GetIssuer(), cancellationToken);
            CheckGrantType(client);
        }

        protected IEnumerable<string> GetScopes(JsonObject previousRequest, HandlerContext context) => GetScopes(previousRequest, context.Request.RequestData);

        protected IEnumerable<string> GetScopes(JsonObject previousRequest, JsonObject newRequest)
        {
            var result = newRequest.GetScopesFromAuthorizationRequest();
            if ((result == null || !result.Any()) && previousRequest != null)
                result = previousRequest.GetScopesFromAuthorizationRequest();
            return result;
        }

        protected IEnumerable<string> GetResources(JsonObject previousRequest, HandlerContext context) => GetResources(previousRequest, context.Request.RequestData);

        protected IEnumerable<string> GetResources(JsonObject previousRequest, JsonObject newRequest)
        {
            var result = newRequest.GetResourcesFromAuthorizationRequest();
            if ((result == null || !result.Any()) && previousRequest != null)
                result = previousRequest.GetResourcesFromAuthorizationRequest();
            return result;
        }

        protected IEnumerable<AuthorizedClaim> GetClaims(JsonObject previousRequest, HandlerContext context) => GetClaims(previousRequest, context.Request.RequestData);

        protected IEnumerable<AuthorizedClaim> GetClaims(JsonObject previousRequest, JsonObject newRequest)
        {
            var result = newRequest.GetClaimsFromAuthorizationRequest();
            if ((result == null || !result.Any()) && previousRequest != null)
                result = previousRequest.GetClaimsFromAuthorizationRequest();
            return result;
        }

        protected ICollection<AuthorizationData> GetAuthorizationDetails(JsonObject previousRequest, JsonObject newRequest)
        {
            var result = newRequest.GetAuthorizationDetailsFromAuthorizationRequest();
            if ((result == null || !result.Any()) && previousRequest != null)
                result = previousRequest.GetAuthorizationDetailsFromAuthorizationRequest();
            return result;
        }

        protected void Issue(JsonObject json, string clientId, string realm)
        {
            if(json.ContainsKey(TokenResponseParameters.RefreshToken))
            {

            }

            if(json.ContainsKey(TokenResponseParameters.IdToken))
            {
                Counters.IssueIdToken(clientId, realm, GrantType);
            }

            if(json.ContainsKey(TokenResponseParameters.AccessToken))
            {

            }
        }

        protected void AddTokenProfile(HandlerContext context)
        {
            if(context.DPOPProof != null)
            {
                context.Response.Add(TokenResponseParameters.TokenType, Constants.DPOPHeaderName);
                return;
            }

            _tokenProfiles.First(t => t.Profile == (context.Client.PreferredTokenProfile ?? _options.DefaultTokenProfile)).Enrich(context);
        }

        protected JsonObject BuildResult(HandlerContext context, IEnumerable<string> scopes)
        {
            return new JsonObject
            {
                [TokenResponseParameters.ExpiresIn] = context.Client?.TokenExpirationTimeInSeconds ?? _options.DefaultTokenExpirationTimeInSeconds,
                [TokenResponseParameters.Scope] = string.Join(" ", scopes)
            };
        }

        private void CheckGrantType(Client client)
        {
            if (client.GrantTypes == null || !client.GrantTypes.Contains(GrantType)) throw new OAuthException(ErrorCodes.INVALID_CLIENT, string.Format(Global.BadClientGrantType, GrantType));
        }

        public static IActionResult BuildError(HttpStatusCode httpStatusCode, string error, string errorMessage)
        {
            var jObj = new JsonObject
            {
                [ErrorResponseParameters.Error] = error,
                [ErrorResponseParameters.ErrorDescription] = errorMessage
            };
            return new ContentResult
            {
                ContentType = "application/json",
                Content = jObj.ToString(),
                StatusCode = (int)httpStatusCode
            };
        }
    }
}