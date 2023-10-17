// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Sms.Services;
using SimpleIdServer.IdServer.Sms.UI.ViewModels;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
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
            IAuthenticationHelper authenticationHelper,
            IClientRepository clientRepository,
            IAmrHelper amrHelper,
            IUserRepository userRepository,
            IUserTransformer userTransformer,
            IBusControl busControl) : base(notificationServices, otpAuthenticators, userAuthenticationService, authenticationSchemeProvider, options, dataProtectionProvider, authenticationHelper, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _configuration = configuration;
        }

        protected override bool IsExternalIdProvidersDisplayed => false;

        protected override string Amr => Constants.AMR;

        protected override string FormattedMessage => GetOptions()?.Message;

        protected override void EnrichViewModel(AuthenticateSmsViewModel viewModel)
        {
        }

        protected override bool TryGetLogin(AmrAuthInfo amr, out string login)
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
