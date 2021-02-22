// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenBankingApi.AccountAccessContents.Commands;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenBankingApi.Resources;
using SimpleIdServer.OpenBankingApi.UI.ViewModels;
using SimpleIdServer.OpenID.Extensions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.UI
{
    [Authorize("IsConnected")]
    public class OpenBankingApiAccountConsentController : Controller
    {
        private readonly IOAuthUserQueryRepository _oauthUserRepository;
        private readonly IOAuthUserCommandRepository _oAuthUserCommandRepository;
        private readonly IOAuthClientQueryRepository _oauthClientRepository;
        private readonly IUserConsentFetcher _userConsentFetcher;
        private readonly IDataProtector _dataProtector;
        private readonly IAccountAccessConsentRepository _accountAccessConsentRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly OAuthHostOptions _oauthHostOptions;
        private readonly ILogger<OpenBankingApiAccountConsentController> _logger;
        private readonly IMediator _mediator;
        private readonly OpenBankingApiOptions _openbankingApiOptions;

        public OpenBankingApiAccountConsentController(
            IOAuthUserQueryRepository oauthUserRepository, 
            IOAuthUserCommandRepository oauthUserCommandRepository, 
            IOAuthClientQueryRepository oauthClientRepository, 
            IUserConsentFetcher userConsentFetcher, 
            IDataProtectionProvider dataProtectionProvider,
            IAccountAccessConsentRepository accountAccessConsentRepository,
            IAccountRepository accountRepository,
            IMediator mediator,
            ILogger<OpenBankingApiAccountConsentController> logger,
            IOptions<OAuthHostOptions> oauthHostOptions,
            IOptions<OpenBankingApiOptions> openbankingApiOptions)
        {
            _oauthUserRepository = oauthUserRepository;
            _oAuthUserCommandRepository = oauthUserCommandRepository;
            _oauthClientRepository = oauthClientRepository;
            _userConsentFetcher = userConsentFetcher;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _accountAccessConsentRepository = accountAccessConsentRepository;
            _accountRepository = accountRepository;
            _mediator = mediator;
            _logger = logger;
            _oauthHostOptions = oauthHostOptions.Value;
            _openbankingApiOptions = openbankingApiOptions.Value;
        }

        public async Task<IActionResult> Index(string returnUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}" });
            }

            try
            {
                var unprotectedUrl = _dataProtector.Unprotect(returnUrl);
                var query = unprotectedUrl.GetQueries().ToJObj();
                var cancellationUrl = query.GetRedirectUriFromAuthorizationRequest();
                if (!string.IsNullOrWhiteSpace(cancellationUrl))
                {
                    var state = query.GetStateFromAuthorizationRequest();
                    if (!string.IsNullOrWhiteSpace(state))
                    {
                        cancellationUrl = QueryHelpers.AddQueryString(cancellationUrl, AuthorizationRequestParameters.State, state);
                    }
                }

                var clientId = query.GetClientIdFromAuthorizationRequest();
                var claims = query.GetClaimsFromAuthorizationRequest();
                var claimName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                var consentId = claims.Single(c => c.Name == _openbankingApiOptions.OpenBankingApiConsentClaimName).Values.First();
                var oauthClient = await _oauthClientRepository.FindOAuthClientById(clientId, cancellationToken);
                var defaultLanguage = CultureInfo.DefaultThreadCurrentUICulture != null ? CultureInfo.DefaultThreadCurrentUICulture.Name : _oauthHostOptions.DefaultCulture;
                var claimDescriptions = new List<string>();
                if (claims != null && claims.Any())
                {
                    claimDescriptions = claims.Select(c => c.Name).ToList();
                }

                var consent = await _accountAccessConsentRepository.Get(consentId, cancellationToken);
                if (consent == null)
                {
                    _logger.LogError($"Account Access Consent '{consentId}' doesn't exist");
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownAccountAccessConsent, consentId)); 
                }

                var accounts = await _accountRepository.GetBySubject(claimName.Value, cancellationToken);
                return View(new OpenBankingApiAccountConsentIndexViewModel(
                    returnUrl,
                    cancellationUrl,
                    oauthClient.ClientNames.GetTranslation(defaultLanguage, oauthClient.ClientId),
                    accounts.Select(a =>new OpenBankingApiAccountViewModel
                    {
                        Id = a.AggregateId,
                        AccountSubType = a.AccountSubType.Name,
                        CashAccounts = a.Accounts.Select(ac => new OpenBankingApiCashAccountViewModel
                        {
                            Identification= ac.Identification,
                            SecondaryIdentification= ac.SecondaryIdentification
                        })
                    }).ToList(),
                    new OpenBankingApiConsentAccountViewModel
                    {
                        Id = consent.AggregateId,
                        ExpirationDateTime = consent.ExpirationDateTime,
                        Permissions = consent.Permissions.Select(p => p.Name)
                    }));
            }
            catch (CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(RejectOpenBankingApiAccountConsentViewModel viewModel, CancellationToken cancellationToken)
        {
            await _mediator.Send(new RejectAccountAccessConsentCommand { ConsentId = viewModel.ConsentId }, cancellationToken);
            return Redirect(viewModel.CancellationUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConfirmOpenBankingApiAccountConsentViewModel viewModel, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new ConfirmAccountAccessConsentCommand(viewModel.ConsentId, viewModel.SelectedAccounts), cancellationToken);
                var unprotectedUrl = _dataProtector.Unprotect(viewModel.ReturnUrl);
                var query = unprotectedUrl.GetQueries().ToJObj();
                var claimName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                var user = await _oauthUserRepository.FindOAuthUserByLogin(claimName.Value, cancellationToken);
                var consent = _userConsentFetcher.FetchFromAuthorizationRequest(user, query);
                if (consent == null)
                {
                    consent = _userConsentFetcher.BuildFromAuthorizationRequest(query);
                    user.Consents.Add(consent);
                    await _oAuthUserCommandRepository.Update(user, cancellationToken);
                    await _oAuthUserCommandRepository.SaveChanges(cancellationToken);
                }

                return Redirect(unprotectedUrl);
            }
            catch (CryptographicException)
            {
                ModelState.AddModelError("invalid_request", "invalid_request");
                return View(viewModel);
            }
        }
    }
}
