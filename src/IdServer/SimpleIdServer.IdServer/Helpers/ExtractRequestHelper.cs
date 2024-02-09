// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers
{
    public interface IExtractRequestHelper
    {
        Task<JsonObject> Extract(string realm, string issuerName, JsonObject jsonObject, Client oauthClient);
        Task<bool> Extract(HandlerContext context);
    }

    public class ExtractRequestHelper : IExtractRequestHelper
    {
        private readonly Infrastructures.IHttpClientFactory _httpClientFactory;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IDistributedCache _distributedCache;
        private readonly IdServerHostOptions _options;

        public ExtractRequestHelper(
            Infrastructures.IHttpClientFactory httpClientFactory,
            IJwtBuilder jwtBuilder, 
            IDistributedCache distributedCache, 
            IOptions<IdServerHostOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _jwtBuilder = jwtBuilder;
            _distributedCache = distributedCache;
            _options = options.Value;
        }

        public async Task<JsonObject> Extract(string realm, string issuerName, JsonObject jsonObject, Client oauthClient)
        {
            var context = new HandlerContext(new HandlerContextRequest(issuerName, null, jsonObject), realm, _options, new HandlerContextResponse());
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

            string json = null;
            if (requestUri.StartsWith(Constants.ParFormatKey) || _options.RequiredPushedAuthorizationRequest)
            {
                var payload = await _distributedCache.GetAsync(requestUri);
                if (payload == null)
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.REQUEST_URI_IS_INVALID);
                context.Request.SetRequestData(JsonObject.Parse(Encoding.UTF8.GetString(payload)).AsObject());
                return true;
            }

            Uri uri;
            if (!Uri.TryCreate(requestUri, UriKind.Absolute, out uri))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_REQUEST_URI_PARAMETER);

            var cleanedUrl = uri.AbsoluteUri.Replace(uri.Fragment, "");
            using (var httpClient = _httpClientFactory.GetHttpClient())
            {
                var httpResult = await httpClient.GetAsync(cleanedUrl);
                json = await httpResult.Content.ReadAsStringAsync();
                return await CheckRequest(context, json);
            }
        }

        protected virtual async Task<bool> CheckRequest(HandlerContext context, string request)
        {
            var openidClient = context.Client;
            var res = await _jwtBuilder.ReadJsonWebToken(context.Realm, request, context.Client, context.Client.RequestObjectSigningAlg, context.Client.RequestObjectEncryptionAlg, CancellationToken.None);
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

            var responseTypes = context.Request.RequestData.GetResponseTypesFromAuthorizationRequest();
            if (responseTypes.Any() && !jwt.GetClaim(AuthorizationRequestParameters.ResponseType).Value.Split(' ').OrderBy(s => s).SequenceEqual(responseTypes.OrderBy(s => s)))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.INVALID_RESPONSE_TYPE_CLAIM);

            if (jwt.GetClaim(AuthorizationRequestParameters.ClientId).Value != context.Client.ClientId)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.INVALID_CLIENT_ID_CLAIM);
        }
    }
}
