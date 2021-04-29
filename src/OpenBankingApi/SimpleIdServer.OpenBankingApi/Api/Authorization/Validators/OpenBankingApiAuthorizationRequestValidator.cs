// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Configuration;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenBankingApi.Resources;
using SimpleIdServer.OpenID.Api.Authorization.Validators;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Api.Authorization.Validators
{
    public class OpenBankingApiAuthorizationRequestValidator : OpenIDAuthorizationRequestValidator
    {
        private readonly OpenBankingApiOptions _options;
        private readonly IAccountAccessConsentRepository _accountAccessConsentRepository;
        private readonly ILogger<OpenBankingApiAuthorizationRequestValidator> _logger;
        private readonly IOAuthWorkflowConverter _workflowConverter;

        public OpenBankingApiAuthorizationRequestValidator(
            IOptions<OpenBankingApiOptions> options,
            IAccountAccessConsentRepository accountAccessConsentRepository,
            ILogger<OpenBankingApiAuthorizationRequestValidator> logger,
            IUserConsentFetcher userConsentFetcher,
            IEnumerable<IOAuthResponseMode> oauthResponseModes,
            IHttpClientFactory httpClientFactory,
            IAmrHelper amrHelper, 
            IJwtParser jwtParser,
            IOAuthWorkflowConverter workflowConverter,
            IExtractRequestHelper extractRequestHelper) : base(userConsentFetcher, oauthResponseModes, httpClientFactory, amrHelper, jwtParser, extractRequestHelper)
        {
            _options = options.Value;
            _accountAccessConsentRepository = accountAccessConsentRepository;
            _logger = logger;
            _workflowConverter = workflowConverter;
        }

        public override async Task Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            var openidClient = (OpenIdClient)context.Client;
            var clientId = context.Request.RequestData.GetClientIdFromAuthorizationRequest();
            var scopes = context.Request.RequestData.GetScopesFromAuthorizationRequest();
            var acrValues = context.Request.RequestData.GetAcrValuesFromAuthorizationRequest();
            var claims = context.Request.RequestData.GetClaimsFromAuthorizationRequest();
            var prompt = context.Request.RequestData.GetPromptFromAuthorizationRequest();
            if (!scopes.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, OAuth.DTOs.AuthorizationRequestParameters.Scope));
            }

            var unsupportedScopes = scopes.Where(s => !context.Client.AllowedScopes.Any(sc => sc.Name == s));
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

            var isValid = await ExtractRequestHelper.Extract(context);
            var isFapiRequest = IsFAPIAuthRequest(context);
            if (!isValid && isFapiRequest)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, string.Join(",", new[] { AuthorizationRequestParameters.Request, AuthorizationRequestParameters.RequestUri })));
            }

            await CommonValidate(context, cancellationToken);
            var responseTypes = context.Request.RequestData.GetResponseTypesFromAuthorizationRequest();
            var nonce = context.Request.RequestData.GetNonceFromAuthorizationRequest();
            var redirectUri = context.Request.RequestData.GetRedirectUriFromAuthorizationRequest();
            var maxAge = context.Request.RequestData.GetMaxAgeFromAuthorizationRequest();
            var idTokenHint = context.Request.RequestData.GetIdTokenHintFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, OAuth.DTOs.AuthorizationRequestParameters.RedirectUri));
            }

            var workflows = _workflowConverter.GetOAuthWorkflows().Where(w => w.WorkflowName == "hybrid");
            if (!workflows.Any(r => r.ResponseTypes.Count() == responseTypes.Count() && responseTypes.OrderBy(s => s).SequenceEqual(r.ResponseTypes.OrderBy(s => s))))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.ONLY_HYBRID_WORKFLOWS_ARE_SUPPORTED);
            }

            if (responseTypes.Contains(TokenResponseParameters.IdToken) && string.IsNullOrWhiteSpace(nonce))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, OpenID.DTOs.AuthorizationRequestParameters.Nonce));
            }

            if (maxAge != null)
            {
                if (DateTime.UtcNow > context.User.GetActiveSession().AuthenticationDateTime.AddSeconds(maxAge.Value))
                {
                    throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, claims, openidClient, cancellationToken));
                }
            }
            else if (openidClient.DefaultMaxAge != null && DateTime.UtcNow > context.User.GetActiveSession().AuthenticationDateTime.AddSeconds(openidClient.DefaultMaxAge.Value))
            {
                throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, claims, openidClient, cancellationToken));
            }

            if (!string.IsNullOrWhiteSpace(idTokenHint))
            {
                var payload = await ExtractIdTokenHint(idTokenHint);
                if (context.User.Id != payload.GetSub())
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, OpenID.ErrorMessages.INVALID_SUBJECT_IDTOKENHINT);
                }

                if (!payload.GetAudiences().Contains(context.Request.IssuerName))
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, OpenID.ErrorMessages.INVALID_AUDIENCE_IDTOKENHINT);
                }
            }

            switch (prompt)
            {
                case PromptParameters.Login:
                    throw new OAuthLoginRequiredException(await GetFirstAmr(acrValues, claims, openidClient, cancellationToken));
                case PromptParameters.Consent:
                    RedirectToConsentView(context);
                    break;
                case PromptParameters.SelectAccount:
                    throw new OAuthSelectAccountRequiredException();
            }

            if (!context.User.HasOpenIDConsent(clientId, scopes, claims) || isFapiRequest)
            {
                RedirectToConsentView(context);
                return;
            }

            if (claims != null)
            {
                var idtokenClaims = claims.Where(cl => cl.Type == AuthorizationRequestClaimTypes.IdToken && cl.IsEssential && Jwt.Constants.USER_CLAIMS.Contains(cl.Name));
                var invalidClaims = idtokenClaims.Where(icl => !context.User.Claims.Any(cl => cl.Type == icl.Name && (icl.Values == null || !icl.Values.Any() || icl.Values.Contains(cl.Value))));
                if (invalidClaims.Any())
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(SimpleIdServer.OpenID.ErrorMessages.INVALID_CLAIMS, string.Join(",", invalidClaims.Select(i => i.Name))));
                }
            }
        }

        protected override void RedirectToConsentView(HandlerContext context)
        {
            RedirectToConsentView(context, false);
        }

        private void RedirectToConsentView(HandlerContext context, bool ignoreDefaultRedirection = true)
        {
            var scopes = context.Request.RequestData.GetScopesFromAuthorizationRequest();
            var claims = context.Request.RequestData.GetClaimsFromAuthorizationRequest();
            var claim = claims.FirstOrDefault(_ => _.Name == _options.OpenBankingApiConsentClaimName);
            if (claim == null)
            {
                base.RedirectToConsentView(context);
                return;
            }

            if (scopes.Contains(_options.AccountsScope))
            {
                var consentId = claim.Values.First();
                var accountAccessConsent = _accountAccessConsentRepository.Get(claim.Values.First(), CancellationToken.None).Result;
                if (accountAccessConsent == null)
                {
                    _logger.LogError($"Account Access Consent '{consentId}' doesn't exist");
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownAccountAccessConsent, consentId));
                }

                if (accountAccessConsent.ClientId != context.Client.ClientId)
                {
                    _logger.LogError($"Client is different");
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.ClientDifferent);
                }

                if (accountAccessConsent.Status == AccountAccessConsentStatus.AwaitingAuthorisation)
                {
                    throw new OAuthUserConsentRequiredException("OpenBankingApiAccountConsent", "Index");
                }

                if (accountAccessConsent.Status == AccountAccessConsentStatus.Rejected)
                {
                    _logger.LogError($"Account Access Consent '{consentId}' has already been rejected");
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.AccountAccessConsentRejected);
                }

                if (accountAccessConsent.Status == AccountAccessConsentStatus.Revoked)
                {
                    _logger.LogError($"Account Access Consent '{consentId}' has already been revoked");
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.AccountAccessConsentRevoked);
                }

                return;
            }

            var s = string.Join(",", scopes);
            _logger.LogError($"consent screen cannot be displayed for the scopes '{s}'");
            throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.ConsentScreenCannotBeDisplayed, s));
        }

        private bool IsFAPIAuthRequest(HandlerContext context)
        {
            var claims = context.Request.RequestData.GetClaimsFromAuthorizationRequest();
            return claims.Any(c => c.Name == _options.OpenBankingApiConsentClaimName);
        }
    }
}
