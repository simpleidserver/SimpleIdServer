// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Exceptions;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Authorization.Validators
{
    public class OpenIDAuthorizationRequestValidator : OAuthAuthorizationRequestValidator
    {
        private readonly IAmrHelper _amrHelper;
        private readonly IJwtParser _jwtParser;
        private readonly IRequestObjectValidator _requestObjectValidator;

        public OpenIDAuthorizationRequestValidator(
            IUserConsentFetcher userConsentFetcher,
            IEnumerable<IOAuthResponseMode> oauthResponseModes,
            IHttpClientFactory httpClientFactory,
            IAmrHelper amrHelper, 
            IJwtParser jwtParser,
            IRequestObjectValidator requestObjectValidator) : base(userConsentFetcher, oauthResponseModes, httpClientFactory)
        {
            _amrHelper = amrHelper;
            _jwtParser = jwtParser;
            _requestObjectValidator = requestObjectValidator;
        }

        public override async Task Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            var openidClient = (OpenIdClient)context.Client;
            var clientId = context.Request.Data.GetClientIdFromAuthorizationRequest();
            var scopes = context.Request.Data.GetScopesFromAuthorizationRequest();
            var acrValues = context.Request.Data.GetAcrValuesFromAuthorizationRequest();
            var prompt = context.Request.Data.GetPromptFromAuthorizationRequest();
            var claims = context.Request.Data.GetClaimsFromAuthorizationRequest();
            if (!scopes.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, OAuth.DTOs.AuthorizationRequestParameters.Scope));
            }

            if (!scopes.Contains(SIDOpenIdConstants.StandardScopes.OpenIdScope.Name))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.OPENID_SCOPE_MISSING);
            }

            var unsupportedScopes = scopes.Where(s => s != SIDOpenIdConstants.StandardScopes.OpenIdScope.Name && !context.Client.AllowedScopes.Any(sc => sc.Name == s));
            if (unsupportedScopes.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.UNSUPPORTED_SCOPES, string.Join(",", unsupportedScopes)));
            }

            if (context.User == null)
            {
                if (prompt == PromptParameters.None)
                {
                    throw new OAuthException(ErrorCodes.LOGIN_REQUIRED, OAuth.ErrorMessages.LOGIN_IS_REQUIRED);
                }

                throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, claims, openidClient, cancellationToken));
            }

            if (!await CheckRequestParameter(context))
            {
                await CheckRequestUriParameter(context);
            }

            await CommonValidate(context, cancellationToken);
            var responseTypes = context.Request.Data.GetResponseTypesFromAuthorizationRequest();
            var nonce = context.Request.Data.GetNonceFromAuthorizationRequest();
            var redirectUri = context.Request.Data.GetRedirectUriFromAuthorizationRequest();
            var maxAge = context.Request.Data.GetMaxAgeFromAuthorizationRequest();
            var idTokenHint = context.Request.Data.GetIdTokenHintFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, OAuth.DTOs.AuthorizationRequestParameters.RedirectUri));
            }

            if (responseTypes.Contains(TokenResponseParameters.IdToken) && string.IsNullOrWhiteSpace(nonce))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, OpenID.DTOs.AuthorizationRequestParameters.Nonce));
            }

            if (maxAge != null)
            {
                if (DateTime.UtcNow > context.User.AuthenticationTime.Value.AddSeconds(maxAge.Value))
                {
                    throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, claims, openidClient, cancellationToken));
                }
            }
            else if (openidClient.DefaultMaxAge != null && DateTime.UtcNow > context.User.AuthenticationTime.Value.AddSeconds(openidClient.DefaultMaxAge.Value))
            {
                throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, claims, openidClient, cancellationToken));
            }

            if (!string.IsNullOrWhiteSpace(idTokenHint))
            {
                var payload = await ExtractIdTokenHint(idTokenHint);
                if (context.User.Id != payload.GetSub())
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_SUBJECT_IDTOKENHINT);
                }

                if (!payload.GetAudiences().Contains(context.Request.IssuerName))
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_AUDIENCE_IDTOKENHINT);
                }
            }

            switch (prompt)
            {
                case PromptParameters.Login:
                    throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, claims, openidClient, cancellationToken));
                case PromptParameters.Consent:
                    RedirectToConsentView(context);
                    break;
                case PromptParameters.SelectAccount :
                    throw new OAuthSelectAccountRequiredException();
            }

            if (!context.User.HasOpenIDConsent(clientId, scopes, claims))
            {
                RedirectToConsentView(context);
            }

            if (claims != null)
            {
                var idtokenClaims = claims.Where(cl => cl.Type == AuthorizationRequestClaimTypes.IdToken && cl.IsEssential && Jwt.Constants.USER_CLAIMS.Contains(cl.Name));
                var invalidClaims = idtokenClaims.Where(icl => !context.User.Claims.Any(cl => cl.Type == icl.Name && (icl.Values == null || !icl.Values.Any() || icl.Values.Contains(cl.Value))));
                if (invalidClaims.Any())
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.INVALID_CLAIMS, string.Join(",", invalidClaims.Select(i => i.Name))));
                }
            }
        }

        protected virtual void RedirectToConsentView(HandlerContext context)
        {
            throw new OAuthUserConsentRequiredException();
        }

        protected Task<bool> CheckRequestParameter(HandlerContext context)
        {
            var request = context.Request.Data.GetRequestFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(request))
            {
                return Task.FromResult(true);
            }

            return CheckRequest(context, request);
        }

        protected async Task<bool> CheckRequestUriParameter(HandlerContext context)
        {
            var requestUri = context.Request.Data.GetRequestUriFromAuthorizationRequest();
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
            var validationResult = await _requestObjectValidator.Validate(request, openidClient, CancellationToken.None);
            context.Request.SetData(JObject.FromObject(validationResult.JwsPayload));
            CheckRequestObject(validationResult.JwsHeader, validationResult.JwsPayload, openidClient, context);
            validationResult.JwsPayload.Add(AuthorizationRequestParameters.Request, request);
            return true;
        }

        protected virtual void CheckRequestObject(JwsHeader header, JwsPayload jwsPayload, OpenIdClient openidClient, HandlerContext context)
        {
            if (jwsPayload == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_JWS_REQUEST_PARAMETER);
            }

            if (
                (!string.IsNullOrWhiteSpace(openidClient.RequestObjectSigningAlg) && header.Alg != openidClient.RequestObjectSigningAlg) ||
                (string.IsNullOrWhiteSpace(openidClient.RequestObjectSigningAlg) && header.Alg != NoneSignHandler.ALG_NAME)
            )
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

            if (!jwsPayload[OAuth.DTOs.AuthorizationRequestParameters.ResponseType].ToString().Split(' ').OrderBy(s => s).SequenceEqual(context.Request.Data.GetResponseTypesFromAuthorizationRequest().OrderBy(s => s)))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.INVALID_RESPONSE_TYPE_CLAIM);
            }

            if (jwsPayload[OAuth.DTOs.AuthorizationRequestParameters.ClientId].ToString() != context.Client.ClientId)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.INVALID_CLIENT_ID_CLAIM);
            }
        }

        protected async Task<string> GetFirstAmr(IEnumerable<string> acrValues, IEnumerable<AuthorizationRequestClaimParameter> claims, OpenIdClient client, CancellationToken cancellationToken)
        {
            var acr = await _amrHelper.FetchDefaultAcr(acrValues, claims, client, cancellationToken);
            if (acr == null)
            {
                return null;
            }

            return acr.AuthenticationMethodReferences.First();
        }

        protected async Task<JwsPayload> ExtractIdTokenHint(string idTokenHint)
        {
            if (!_jwtParser.IsJwsToken(idTokenHint) && !_jwtParser.IsJweToken(idTokenHint))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_IDTOKENHINT);
            }

            if (_jwtParser.IsJweToken(idTokenHint))
            {
                idTokenHint = await _jwtParser.Decrypt(idTokenHint);
                if (string.IsNullOrWhiteSpace(idTokenHint))
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_IDTOKENHINT);
                }
            }

            return await _jwtParser.Unsign(idTokenHint);
        }
    }
}