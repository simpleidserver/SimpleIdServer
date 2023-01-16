// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers
{
    public interface IExtractRequestHelper
    {
        Task<JsonObject> Extract(string issuerName, JsonObject jsonObject, Client oauthClient);
        Task<bool> Extract(HandlerContext context);
    }

    public class ExtractRequestHelper : IExtractRequestHelper
    {
        private readonly IJwtBuilder _jwtBuilder;

        public ExtractRequestHelper(IJwtBuilder jwtBuilder)
        {
            _jwtBuilder = jwtBuilder;
        }

        public async Task<JsonObject> Extract(string issuerName, JsonObject jsonObject, Client oauthClient)
        {
            var context = new HandlerContext(new HandlerContextRequest(issuerName, null, jsonObject), new HandlerContextResponse());
            context.SetClient(oauthClient);
            await Extract(context);
            return context.Request.RequestData;
        }

        public async Task<bool> Extract(HandlerContext context)
        {
            if (!await CheckRequestParameter(context))
                return await CheckRequestUriParameter(context);

            return true;
        }

        protected Task<bool> CheckRequestParameter(HandlerContext context)
        {
            var request = context.Request.RequestData.GetRequestFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(request))
                return Task.FromResult(false);

            return CheckRequest(context, request);
        }

        protected async Task<bool> CheckRequestUriParameter(HandlerContext context)
        {
            var requestUri = context.Request.RequestData.GetRequestUriFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(requestUri))
                return false;

            Uri uri;
            if (!Uri.TryCreate(requestUri, UriKind.Absolute, out uri))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_REQUEST_URI_PARAMETER);

            var cleanedUrl = uri.AbsoluteUri.Replace(uri.Fragment, "");
            using (var httpClient = new HttpClient())
            {
                var httpResult = await httpClient.GetAsync(cleanedUrl);
                var json = await httpResult.Content.ReadAsStringAsync();
                return await CheckRequest(context, json);
            }
        }

        protected virtual async Task<bool> CheckRequest(HandlerContext context, string request)
        {
            var openidClient = context.Client;
            var res = await _jwtBuilder.ReadJsonWebToken(request, context.Client, context.Client.RequestObjectSigningAlg, context.Client.RequestObjectEncryptionAlg, CancellationToken.None);
            CheckRequestObject(res.Jwt, openidClient, context);
            context.Request.SetRequestData(res.Jwt.GetClaimJson());
            return true;
        }

        protected virtual void CheckRequestObject(JsonWebToken jwt, Client client, HandlerContext context)
        {
            if (jwt == null)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_JWS_REQUEST_PARAMETER);

            if (!string.IsNullOrWhiteSpace(client.RequestObjectSigningAlg) && jwt.Alg != client.RequestObjectSigningAlg)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.INVALID_SIGNATURE_ALG);

            if (!jwt.TryGetClaim(AuthorizationRequestParameters.ResponseType, out Claim c))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.MISSING_RESPONSE_TYPE_CLAIM);

            if (!jwt.TryGetClaim(AuthorizationRequestParameters.ClientId, out Claim cl))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.MISSING_CLIENT_ID_CLAIM);

            if (!jwt.GetClaim(AuthorizationRequestParameters.ResponseType).Value.Split(' ').OrderBy(s => s).SequenceEqual(context.Request.RequestData.GetResponseTypesFromAuthorizationRequest().OrderBy(s => s)))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.INVALID_RESPONSE_TYPE_CLAIM);

            if (jwt.GetClaim(AuthorizationRequestParameters.ClientId).Value != context.Client.ClientId)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.INVALID_CLIENT_ID_CLAIM);
        }
    }
}
