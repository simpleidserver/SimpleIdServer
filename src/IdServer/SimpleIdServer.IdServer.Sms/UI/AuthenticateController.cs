// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Sms.UI.ViewModels;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;

namespace SimpleIdServer.IdServer.Sms.UI
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseOTPAuthenticateController<AuthenticateSmsViewModel>
    {
        private readonly IdServerSmsOptions _options;

        public AuthenticateController(
            IEnumerable<IUserNotificationService> notificationServices,
            IEnumerable<IOTPAuthenticator> otpAuthenticators,
            IOptions<IdServerSmsOptions> smsOptions,
            IOptions<IdServerHostOptions> options,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IDataProtectionProvider dataProtectionProvider,
            IClientRepository clientRepository,
            IAmrHelper amrHelper,
            IUserRepository userRepository,
            IUserTransformer userTransformer,
            IBusControl busControl) : base(notificationServices, otpAuthenticators, options, authenticationSchemeProvider, dataProtectionProvider, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _options = smsOptions.Value;
        }

        protected override bool IsExternalIdProvidersDisplayed => false;

        protected override string Amr => Constants.AMR;

        protected override string FormattedMessage => _options.Message;

        protected override bool TryGetLogin(User user, out string login)
        {
            login = null;
            if (user == null) return false;
            var cl = user.OAuthUserClaims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.PhoneNumber);
            if (cl == null || string.IsNullOrWhiteSpace(cl.Value)) return false;
            login = cl.Value;
            return true;
        }

        protected override void EnrichViewModel(AuthenticateSmsViewModel viewModel, User user)
        {

        }

        protected override async Task<User> AuthenticateUser(string login, string realm, CancellationToken cancellationToken)
        {
            var user = await UserRepository.Query()
                .Include(u => u.Realms)
                .Include(u => u.Credentials)
                .Include(u => u.OAuthUserClaims)
                .FirstOrDefaultAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.OAuthUserClaims.Any(c => c.Name == JwtRegisteredClaimNames.PhoneNumber && c.Value == login), cancellationToken);
            return user;
        }
    }
}
