// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Stores;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Authorization.Validators
{
    public class OpenIDAuthorizationRequestValidator : OAuthAuthorizationRequestValidator
    {
        private readonly IAmrHelper _amrHelper;
        private readonly IExtractRequestHelper _extractRequestHelper;
        private readonly IKeyStore _keyStore;

        public OpenIDAuthorizationRequestValidator(
            IUserConsentFetcher userConsentFetcher,
            IEnumerable<IOAuthResponseMode> oauthResponseModes,
            IClientHelper clientHelper,
            IAmrHelper amrHelper,
            IExtractRequestHelper extractRequestHelper,
            IKeyStore keyStore) : base(userConsentFetcher, oauthResponseModes, clientHelper)
        {
            _amrHelper = amrHelper;
            _extractRequestHelper = extractRequestHelper;
            _keyStore = keyStore;
        }

        protected IExtractRequestHelper ExtractRequestHelper => _extractRequestHelper;

        public override async Task Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            var openidClient = context.Client;
            var clientId = context.Request.RequestData.GetClientIdFromAuthorizationRequest();
            var scopes = context.Request.RequestData.GetScopesFromAuthorizationRequest();
            var acrValues = context.Request.RequestData.GetAcrValuesFromAuthorizationRequest();
            var prompt = context.Request.RequestData.GetPromptFromAuthorizationRequest();
            var claims = context.Request.RequestData.GetClaimsFromAuthorizationRequest();
            if (!scopes.Any())
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, OAuth.DTOs.AuthorizationRequestParameters.Scope));

            if (!scopes.Contains(SIDOpenIdConstants.StandardScopes.OpenIdScope.Name))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.OPENID_SCOPE_MISSING);

            var unsupportedScopes = scopes.Where(s => s != SIDOpenIdConstants.StandardScopes.OpenIdScope.Name && !context.Client.Scopes.Any(sc => sc.Name == s));
            if (unsupportedScopes.Any())
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.UNSUPPORTED_SCOPES, string.Join(",", unsupportedScopes)));

            if (context.User == null)
            {
                if (prompt == PromptParameters.None)
                    throw new OAuthException(ErrorCodes.LOGIN_REQUIRED, OAuth.ErrorMessages.LOGIN_IS_REQUIRED);

                throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, claims, openidClient, cancellationToken));
            }

            var activeSession = context.User.ActiveSession;
            if (activeSession == null)
                throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, claims, openidClient, cancellationToken), true);

            await _extractRequestHelper.Extract(context);
            await CommonValidate(context, cancellationToken);
            var responseTypes = context.Request.RequestData.GetResponseTypesFromAuthorizationRequest();
            var nonce = context.Request.RequestData.GetNonceFromAuthorizationRequest();
            var redirectUri = context.Request.RequestData.GetRedirectUriFromAuthorizationRequest();
            var maxAge = context.Request.RequestData.GetMaxAgeFromAuthorizationRequest();
            var idTokenHint = context.Request.RequestData.GetIdTokenHintFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(redirectUri))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, OAuth.DTOs.AuthorizationRequestParameters.RedirectUri));

            if (responseTypes.Contains(TokenResponseParameters.IdToken) && string.IsNullOrWhiteSpace(nonce))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, OpenID.DTOs.AuthorizationRequestParameters.Nonce));

            if (maxAge != null)
            {
                if (DateTime.UtcNow > activeSession.AuthenticationDateTime.AddSeconds(maxAge.Value))
                    throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, claims, openidClient, cancellationToken));
            }
            else if (openidClient.GetDefaultMaxAge() != null && DateTime.UtcNow > activeSession.AuthenticationDateTime.AddSeconds(openidClient.GetDefaultMaxAge().Value))
                throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, claims, openidClient, cancellationToken));

            if (!string.IsNullOrWhiteSpace(idTokenHint))
            {
                var payload = ExtractIdTokenHint(idTokenHint, cancellationToken);
                if (context.User.Id != payload.Subject)
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_SUBJECT_IDTOKENHINT);

                if (!payload.Audiences.Contains(context.Request.IssuerName))
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_AUDIENCE_IDTOKENHINT);
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
                var idtokenClaims = claims.Where(cl => cl.Type == AuthorizationRequestClaimTypes.IdToken && cl.IsEssential && SIDOpenIdConstants.AllUserClaims.Contains(cl.Name));
                var invalidClaims = idtokenClaims.Where(icl => !context.User.Claims.Any(cl => cl.Type == icl.Name && (icl.Values == null || !icl.Values.Any() || icl.Values.Contains(cl.Value))));
                if (invalidClaims.Any())
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.INVALID_CLAIMS, string.Join(",", invalidClaims.Select(i => i.Name))));
                }
            }
        }

        protected virtual void RedirectToConsentView(HandlerContext context)
        {
            if (context.Client.IsConsentDisabled) return;
            throw new OAuthUserConsentRequiredException();
        }

        protected async Task<string> GetFirstAmr(IEnumerable<string> acrValues, IEnumerable<AuthorizationRequestClaimParameter> claims, Client client, CancellationToken cancellationToken)
        {
            var acr = await _amrHelper.FetchDefaultAcr(acrValues, claims, client, cancellationToken);
            if (acr == null)
                return null;

            return acr.AuthenticationMethodReferences.First();
        }

        protected JsonWebToken ExtractIdTokenHint(string idTokenHint, CancellationToken cancellationToken)
        {
            var handler = new JsonWebTokenHandler();
            if (handler.CanReadToken(idTokenHint))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_IDTOKENHINT);

            // Note : The Client MAY re-encrypt the signed ID token to the Authentication Server using a key that enables the server to decrypt the ID Token,
            // and use the re-encrypted ID token as the id_token_hint value.
            var token = handler.ReadJsonWebToken(idTokenHint);
            if(token.IsEncrypted)
            {
                var encryptionKeys = _keyStore.GetAllEncryptingKeys();
                var encryptionKey = encryptionKeys.FirstOrDefault(e => token.Kid == e.Key.KeyId);
                if (encryptionKey == null)
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_JSON_WEBKEY, token.Kid));
                idTokenHint = handler.DecryptToken(token, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = false,
                    TokenDecryptionKey = encryptionKey.Key
                });
                token = handler.ReadJsonWebToken(idTokenHint);
            }

            var signKeys = _keyStore.GetAllSigningKeys();
            var sigKey = signKeys.FirstOrDefault(k => k.Kid == token.Kid);
            if (sigKey == null)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_JSON_WEBKEY, token.Kid));
            var validationResult = handler.ValidateToken(idTokenHint, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                IssuerSigningKey = sigKey.Key
            });
            if (!validationResult.IsValid)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.BAD_ID_TOKEN_HINT_SIG);
            return token;
        }
    }
}