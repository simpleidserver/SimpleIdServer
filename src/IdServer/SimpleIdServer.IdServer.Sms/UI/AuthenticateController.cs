// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using MassTransit;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Sms.Services;
using SimpleIdServer.IdServer.Sms.UI.ViewModels;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Infrastructures;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Sms.UI
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseOTPAuthenticateController<AuthenticateSmsViewModel>
    {
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            IConfiguration configuration,
            IEnumerable<IUserNotificationService> notificationServices,
            IEnumerable<IOTPAuthenticator> otpAuthenticators,
            IUserSmsAuthenticationService userAuthenticationService,
            IOptions<IdServerHostOptions> options,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IDataProtectionProvider dataProtectionProvider,
            ITransactionBuilder transactionBuilder,
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder,
            IAuthenticationHelper authenticationHelper,
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
            IOptions<FormBuilderOptions> formBuilderOptions) : base(configuration, notificationServices, otpAuthenticators, userAuthenticationService, authenticationSchemeProvider, options, dataProtectionProvider, authenticationHelper, clientRepository, amrHelper, userRepository, userSessionRepository, userTransformer, tokenRepository, transactionBuilder, jwtBuilder, busControl, antiforgery, authenticationContextClassReferenceRepository, sessionManager, workflowStore, formStore, languageRepository, formBuilderOptions)
        {
            _configuration = configuration;
        }

        protected override bool IsExternalIdProvidersDisplayed => false;

        protected override string Amr => Constants.AMR;

        protected override string FormattedMessage => GetOptions()?.Message;

        protected override void EnrichViewModel(AuthenticateSmsViewModel viewModel)
        {
        }

        protected override bool TryGetLogin(AcrAuthInfo amr, out string login)
        {
            login = null;
            if (amr == null || !amr.Claims.Any(cl => cl.Key == JwtRegisteredClaimNames.PhoneNumber)) return false;
            var cl = amr.Claims.Single(c => c.Key == JwtRegisteredClaimNames.PhoneNumber);
            login = cl.Value;
            return true;
        }

        private IdServerSmsOptions GetOptions()
        {
            var section = _configuration.GetSection(typeof(IdServerSmsOptions).Name);
            return section.Get<IdServerSmsOptions>();
        }
    }
}
