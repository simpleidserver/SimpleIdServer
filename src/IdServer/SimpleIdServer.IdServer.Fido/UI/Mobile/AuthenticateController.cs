// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using MassTransit;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Captcha;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Fido.Apis;
using SimpleIdServer.IdServer.Fido.Services;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Infrastructures;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Fido.UI.Mobile
{
    [Area(Constants.MobileAMR)]
    public class AuthenticateController : BaseAuthenticationMethodController<AuthenticateMobileViewModel>
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _distributedCache;

        public AuthenticateController(
            ITemplateStore templateStore,
            IConfiguration configuration, 
            IAuthenticationHelper authenticationHelper,
            IMobileAuthenticationService userAuthenticationService,
            IDistributedCache distributedCache, 
            IOptions<IdServerHostOptions> options, 
            IAuthenticationSchemeProvider authenticationSchemeProvider, 
            IDataProtectionProvider dataProtectionProvider,
            ITokenRepository tokenRepository,
            ITransactionBuilder transactionBuilder,
            IJwtBuilder jwtBuilder,
            IClientRepository clientRepository, 
            IAmrHelper amrHelper, 
            IUserRepository userRepository,
            IUserSessionResitory userSessionRepository,
            IUserTransformer userTransformer, 
            IBusControl busControl,
            IAntiforgery antiforgery,
            IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository,
            ISessionManager sessionManager, 
            IWorkflowStore workflowStore,
            IFormStore formStore,
            ILanguageRepository languageRepository,
            IAcrHelper acrHelper,
            IWorkflowHelper workflowHelper,
            ICaptchaValidatorFactory captchaValidatorFactory,
            IOptions<FormBuilderOptions> formBuilderOptions) : base(templateStore, configuration, options, authenticationSchemeProvider, userAuthenticationService, dataProtectionProvider, tokenRepository, transactionBuilder, jwtBuilder, authenticationHelper, clientRepository, amrHelper, userRepository, userSessionRepository, userTransformer, busControl, antiforgery, authenticationContextClassReferenceRepository, sessionManager, workflowStore, formStore, languageRepository, acrHelper, workflowHelper, captchaValidatorFactory, formBuilderOptions)
        {
            _configuration = configuration;
            _distributedCache= distributedCache;
        }

        protected override string Amr => Constants.MobileAMR;

        protected override bool IsExternalIdProvidersDisplayed => false;

        protected override bool TryGetLogin(AcrAuthInfo amr, out string login)
        {
            login = null;
            if (amr == null || string.IsNullOrWhiteSpace(amr.Login)) return false;
            login = amr.Login;
            return true;
        }

        protected override Task<UserAuthenticationResult> CustomAuthenticate(string prefix, string authenticatedUserId, AuthenticateMobileViewModel viewModel, CancellationToken cancellationToken)
        {
            return Task.FromResult(UserAuthenticationResult.Ok());
        }

        protected override void EnrichViewModel(AuthenticateMobileViewModel viewModel)
        {
            var options = GetMobileOptions();
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var realm = "/";
            if (!string.IsNullOrWhiteSpace(viewModel.Realm))
                realm = $"/{viewModel.Realm}/";
            viewModel.BeginLoginUrl = $"{issuer}{realm}{Constants.EndPoints.BeginQRCodeLogin}";
            viewModel.LoginStatusUrl = $"{issuer}{realm}{Constants.EndPoints.LoginStatus}";
        }

        protected async Task<ValidationStatus> ValidateCredentials(AuthenticateMobileViewModel viewModel, User user, CancellationToken cancellationToken)
        {
            var session = await _distributedCache.GetStringAsync(viewModel.SessionId, cancellationToken);
            if (string.IsNullOrWhiteSpace(session))
            {
                ModelState.AddModelError("unknown_session", "unknown_session");
                return ValidationStatus.NOCONTENT;
            }

            var sessionRecord = JsonSerializer.Deserialize<AuthenticationSessionRecord>(session);
            if (!sessionRecord.IsValidated)
            {
                ModelState.AddModelError("session_not_validated", "session_not_validated");
                return ValidationStatus.NOCONTENT;
            }

            return ValidationStatus.AUTHENTICATE;
        }

        private MobileOptions GetMobileOptions()
        {
            var section = _configuration.GetSection(typeof(MobileOptions).Name);
            return section.Get<MobileOptions>() ?? new MobileOptions();
        }
    }
}
