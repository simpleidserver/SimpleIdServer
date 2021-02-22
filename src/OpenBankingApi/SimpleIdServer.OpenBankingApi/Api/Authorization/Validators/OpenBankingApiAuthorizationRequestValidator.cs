// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenBankingApi.Resources;
using SimpleIdServer.OpenID.Api.Authorization.Validators;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
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

        public OpenBankingApiAuthorizationRequestValidator(
            IOptions<OpenBankingApiOptions> options,
            ILogger<OpenBankingApiAuthorizationRequestValidator> logger,
            IAccountAccessConsentRepository accountAccessConsentRepository,
            IAmrHelper amrHelper, 
            IJwtParser jwtParser) : base(amrHelper, jwtParser)
        {
            _options = options.Value;
            _logger = logger;
            _accountAccessConsentRepository = accountAccessConsentRepository;
        }

        public override async Task Validate(HandlerContext context)
        {
            await base.Validate(context);
            RedirectToConsentView(context, true);
        }

        protected override void RedirectToConsentView(HandlerContext context)
        {
            RedirectToConsentView(context, false);
        }

        private void RedirectToConsentView(HandlerContext context, bool ignoreDefaultRedirection = true)
        {
            var scopes = context.Request.Data.GetScopesFromAuthorizationRequest();
            var claims = context.Request.Data.GetClaimsFromAuthorizationRequest();
            var claim = claims.FirstOrDefault(_ => _.Name == _options.OpenBankingApiConsentClaimName);
            if (claim == null)
            {
                if (ignoreDefaultRedirection)
                {
                    return;
                }

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

                if (accountAccessConsent.Status == AccountAccessConsentStatus.AwaitingAuthorisation)
                {
                    throw new OAuthUserConsentRequiredException("OpenBankingApiAccountConsent", "Index");
                }

                if (accountAccessConsent.Status == AccountAccessConsentStatus.Rejected)
                {
                    _logger.LogError($"Account Access Consent '{consentId}' has already been rejected");
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.AccountAccessConsentRejected, consentId));
                }

                if (accountAccessConsent.Status == AccountAccessConsentStatus.Revoked)
                {
                    _logger.LogError($"Account Access Consent '{consentId}' has already been revoked");
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.AccountAccessConsentRevoked, consentId));
                }

                return;
            }

            var s = string.Join(",", scopes);
            _logger.LogError($"consent screen cannot be displayed for the scopes '{s}'");
            throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.ConsentScreenCannotBeDisplayed, s));
        }
    }
}
