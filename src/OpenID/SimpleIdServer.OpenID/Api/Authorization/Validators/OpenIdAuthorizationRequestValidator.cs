// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
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
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Authorization.Validators
{
    public class OpenIDAuthorizationRequestValidator : IAuthorizationRequestValidator
    {
        private readonly IAmrHelper _amrHelper;
        private readonly IJwtParser _jwtParser;

        public OpenIDAuthorizationRequestValidator(IAmrHelper amrHelper, IJwtParser jwtParser)
        {
            _amrHelper = amrHelper;
            _jwtParser = jwtParser;
        }

        public async Task Validate(HandlerContext context)
        {
            var openidClient = (OpenIdClient)context.Client;
            var clientId = context.Request.Data.GetClientIdFromAuthorizationRequest();
            var scopes = context.Request.Data.GetScopesFromAuthorizationRequest();
            var acrValues = context.Request.Data.GetAcrValuesFromAuthorizationRequest();
            var prompt = context.Request.Data.GetPromptFromAuthorizationRequest();
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

                throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, openidClient));
            }

            if (!await CheckRequestParameter(context))
            {
                await CheckRequestUriParameter(context);
            }

            var redirectUri = context.Request.Data.GetRedirectUriFromAuthorizationRequest();
            var claims = context.Request.Data.GetClaimsFromAuthorizationRequest();
            var maxAge = context.Request.Data.GetMaxAgeFromAuthorizationRequest();
            var idTokenHint = context.Request.Data.GetIdTokenHintFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, OAuth.DTOs.AuthorizationRequestParameters.RedirectUri));
            }

            if (maxAge != null)
            {
                if (DateTime.UtcNow > context.Request.AuthDateTime.Value.AddSeconds(maxAge.Value))
                {
                    throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, openidClient));
                }
            }
            else if (openidClient.DefaultMaxAge != null && DateTime.UtcNow > context.Request.AuthDateTime.Value.AddSeconds(openidClient.DefaultMaxAge.Value))
            {
                throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, openidClient));
            }

            if (string.IsNullOrWhiteSpace(idTokenHint) && prompt == PromptParameters.None)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.IdTokenHint));
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
                    throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, openidClient));
                case PromptParameters.Consent:
                    throw new OAuthUserConsentRequiredException();
                case PromptParameters.SelectAccount :
                    throw new OAuthSelectAccountRequiredException();
            }

            if (!context.User.HasOpenIDConsent(clientId, scopes, claims))
            {
                throw new OAuthUserConsentRequiredException();
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

        private Task<bool> CheckRequestParameter(HandlerContext context)
        {
            var request = context.Request.Data.GetRequestFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(request))
            {
                return Task.FromResult(false);
            }

            return CheckRequest(context, request);
        }

        private async Task<bool> CheckRequestUriParameter(HandlerContext context)
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

            using (var httpClient = new HttpClient())
            {
                var httpResult = await httpClient.GetAsync(uri);
                var json = await httpResult.Content.ReadAsStringAsync();
                return await CheckRequest(context, json);
            }
        }

        private async Task<bool> CheckRequest(HandlerContext context, string request)
        {
            var openidClient = (OpenIdClient)context.Client;
            if (!_jwtParser.IsJwsToken(request) && !_jwtParser.IsJweToken(request))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_REQUEST_PARAMETER);
            }

            var jws = request;
            if (_jwtParser.IsJweToken(request))
            {
                jws = await _jwtParser.Decrypt(jws);
                if (string.IsNullOrWhiteSpace(jws))
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_JWE_REQUEST_PARAMETER);
                }
            }

            JwsHeader header = null;
            try
            {
                header = _jwtParser.ExtractJwsHeader(jws);
            }
            catch (InvalidOperationException)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_JWS_REQUEST_PARAMETER);
            }

            if (header.Alg != openidClient.RequestObjectSigningAlg)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_SIGNATURE_ALG);
            }

            var jwsPayload = await _jwtParser.Unsign(jws, context.Client);
            if (jwsPayload == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_JWS_REQUEST_PARAMETER);
            }

            var issuer = jwsPayload.GetIssuer();
            var audiences = jwsPayload.GetAudiences();
            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_ISSUER_CLAIM);
            }

            if (issuer != context.Client.ClientId)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_ISSUER_CLAIM);
            }

            if (!audiences.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_AUD_CLAIM);
            }

            if (!jwsPayload.ContainsKey(OAuth.DTOs.AuthorizationRequestParameters.ResponseType))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_RESPONSE_TYPE_CLAIM);
            }

            if (!jwsPayload.ContainsKey(OAuth.DTOs.AuthorizationRequestParameters.ClientId))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_CLIENT_ID_CLAIM);
            }

            if (!jwsPayload[OAuth.DTOs.AuthorizationRequestParameters.ResponseType].ToString().Split(' ').OrderBy(s => s).SequenceEqual(context.Request.Data.GetResponseTypesFromAuthorizationRequest()))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_RESPONSE_TYPE_CLAIM);
            }

            if (jwsPayload[OAuth.DTOs.AuthorizationRequestParameters.ClientId].ToString() != context.Client.ClientId)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_CLIENT_ID_CLAIM);
            }

            context.Request.SetData(JObject.FromObject(jwsPayload));
            return true;
        }

        private async Task<string> GetFirstAmr(IEnumerable<string> acrValues, OpenIdClient client)
        {
            var acr = await _amrHelper.FetchDefaultAcr(acrValues, client);
            if (acr == null)
            {
                return null;
            }

            return acr.AuthenticationMethodReferences.First();
        }

        private async Task<JwsPayload> ExtractIdTokenHint(string idTokenHint)
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