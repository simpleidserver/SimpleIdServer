// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Email.Services;
using SimpleIdServer.IdServer.Email.UI.ViewModels;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Email.UI
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseOTPAuthenticateController<AuthenticateEmailViewModel>
    {
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            IConfiguration configuration,
            IEnumerable<IUserNotificationService> notificationServices,
            IEnumerable<IOTPAuthenticator> otpAuthenticators,
            IUserEmailAuthenticationService userAuthenticationService,
            IOptions<IdServerHostOptions> options,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IDataProtectionProvider dataProtectionProvider,
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder,
            IAuthenticationHelper authenticationHelper,
            IClientRepository clientRepository, 
            IAmrHelper amrHelper,
            IUserRepository userRepository, 
            IUserSessionResitory userSessionRepository,
            IUserTransformer userTransformer, 
            IBusControl busControl,
            IAntiforgery antiforgery) : base(notificationServices, otpAuthenticators, userAuthenticationService, authenticationSchemeProvider, options, dataProtectionProvider, authenticationHelper, clientRepository, amrHelper, userRepository, userSessionRepository, userTransformer, tokenRepository, jwtBuilder, busControl, antiforgery)
        {
            _configuration = configuration;
        }

        protected override bool IsExternalIdProvidersDisplayed => false;

        protected override string Amr => Constants.AMR;

        protected override string FormattedMessage => GetOptions()?.HttpBody;

        protected override bool TryGetLogin(AmrAuthInfo amr, out string login)
        {
            login = null;
            if (amr == null || string.IsNullOrWhiteSpace(amr.Email)) return false;
            login = amr.Email;
            return true;
        }

        protected override void EnrichViewModel(AuthenticateEmailViewModel viewModel)
        {
        }

        private IdServerEmailOptions GetOptions()
        {
            var section = _configuration.GetSection(typeof(IdServerEmailOptions).Name);
            return section.Get<IdServerEmailOptions>();
        }
    }
}
