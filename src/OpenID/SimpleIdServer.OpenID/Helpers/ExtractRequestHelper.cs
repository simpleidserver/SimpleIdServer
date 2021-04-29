// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Extensions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Helpers
{
    public class ExtractRequestHelper: IExtractRequestHelper
    {
        private readonly IRequestObjectValidator _requestObjectValidator;

        public ExtractRequestHelper(IRequestObjectValidator requestObjectValidator)
        {
            _requestObjectValidator = requestObjectValidator;
        }

        public async Task<JObject> Extract(string issuerName, JObject jObj, OAuthClient oauthClient)
        {
            var context = new HandlerContext(new HandlerContextRequest(issuerName, null, jObj), new HandlerContextResponse());
            context.SetClient(oauthClient);
            await Extract(context);
            return context.Request.RequestData;
        }

        public async Task<bool> Extract(HandlerContext context)
        {
            if (!await CheckRequestParameter(context))
            {
                return await CheckRequestUriParameter(context);
            }

            return true;
        }

        protected Task<bool> CheckRequestParameter(HandlerContext context)
        {
            var request = context.Request.RequestData.GetRequestFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(request))
            {
                return Task.FromResult(false);
            }

            return CheckRequest(context, request);
        }

        protected async Task<bool> CheckRequestUriParameter(HandlerContext context)
        {
            var requestUri = context.Request.RequestData.GetRequestUriFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(requestUri))
            {
                return false;
            }

            Uri uri;
            if (!Uri.TryCreate(requestUri, UriKind.Absolute, out uri))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_REQUEST_URI_PARAMETER);
            }

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
            var openidClient = (OpenIdClient)context.Client;
            var validationResult = await _requestObjectValidator.Validate(request, openidClient, CancellationToken.None, ErrorCodes.INVALID_REQUEST_OBJECT);
            context.Request.SetRequestData(JObject.FromObject(validationResult.JwsPayload));
            CheckRequestObject(validationResult.JwsHeader, validationResult.JwsPayload, openidClient, context);
            validationResult.JwsPayload.Add(AuthorizationRequestParameters.Request, request);
            return true;
        }

        protected virtual void CheckRequestObject(JwsHeader header, JwsPayload jwsPayload, OpenIdClient openidClient, HandlerContext context)
        {
            if (jwsPayload == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, OAuth.ErrorMessages.INVALID_JWS_REQUEST_PARAMETER);
            }

            if (!string.IsNullOrWhiteSpace(openidClient.RequestObjectSigningAlg) && header.Alg != openidClient.RequestObjectSigningAlg)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.INVALID_SIGNATURE_ALG);
            }

            if (!jwsPayload.ContainsKey(OAuth.DTOs.AuthorizationRequestParameters.ResponseType))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.MISSING_RESPONSE_TYPE_CLAIM);
            }

            if (!jwsPayload.ContainsKey(OAuth.DTOs.AuthorizationRequestParameters.ClientId))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.MISSING_CLIENT_ID_CLAIM);
            }

            if (!jwsPayload[OAuth.DTOs.AuthorizationRequestParameters.ResponseType].ToString().Split(' ').OrderBy(s => s).SequenceEqual(context.Request.RequestData.GetResponseTypesFromAuthorizationRequest().OrderBy(s => s)))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.INVALID_RESPONSE_TYPE_CLAIM);
            }

            if (jwsPayload[OAuth.DTOs.AuthorizationRequestParameters.ClientId].ToString() != context.Client.ClientId)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.INVALID_CLIENT_ID_CLAIM);
            }
        }
    }
}
