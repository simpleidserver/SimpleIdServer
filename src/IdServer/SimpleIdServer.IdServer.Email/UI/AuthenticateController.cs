// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Email.UI.ViewModels;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;

namespace SimpleIdServer.IdServer.Email.UI
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseOTPAuthenticateController<AuthenticateEmailViewModel>
    {
        private readonly IdServerEmailOptions _options;

        public AuthenticateController(
            IEnumerable<IUserNotificationService> notificationServices,
            IEnumerable<IOTPAuthenticator> otpAuthenticators,
            IOptions<IdServerHostOptions> options,
            IOptions<IdServerEmailOptions> emailOptions,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IDataProtectionProvider dataProtectionProvider, 
            IClientRepository clientRepository, 
            IAmrHelper amrHelper,
            IUserRepository userRepository, 
            IUserTransformer userTransformer, 
            IBusControl busControl) : base(notificationServices, otpAuthenticators, options, authenticationSchemeProvider, dataProtectionProvider, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _options = emailOptions.Value;
        }

        protected override bool IsExternalIdProvidersDisplayed => false;

        protected override string Amr => Constants.AMR;

        protected override string FormattedMessage => _options.HttpBody;

        protected override bool TryGetLogin(User user, out string login)
        {
            login = null;
            if (user == null || string.IsNullOrWhiteSpace(user.Email)) return false;
            login = user.Email;
            return true;
        }

        protected override void EnrichViewModel(AuthenticateEmailViewModel viewModel, User user)
        {

        }

        protected override async Task<User> AuthenticateUser(string login, string realm, CancellationToken cancellationToken)
        {
            var user = await UserRepository.Query()
                .Include(u => u.Realms)
                .Include(u => u.Credentials)
                .Include(u => u.OAuthUserClaims)
                .FirstOrDefaultAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.Email == login, cancellationToken);
            return user;
        }
    }
}
