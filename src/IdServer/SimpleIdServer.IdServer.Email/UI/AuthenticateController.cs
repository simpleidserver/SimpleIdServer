// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            IConfiguration configuration,
            IEnumerable<IUserNotificationService> notificationServices,
            IEnumerable<IOTPAuthenticator> otpAuthenticators,
            IOptions<IdServerHostOptions> options,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IDataProtectionProvider dataProtectionProvider, 
            IClientRepository clientRepository, 
            IAmrHelper amrHelper,
            IUserRepository userRepository, 
            IUserTransformer userTransformer, 
            IBusControl busControl) : base(notificationServices, otpAuthenticators, options, authenticationSchemeProvider, dataProtectionProvider, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _configuration = configuration;
        }

        protected override bool IsExternalIdProvidersDisplayed => false;

        protected override string Amr => Constants.AMR;

        protected override string FormattedMessage => GetOptions()?.HttpBody;

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

        private IdServerEmailOptions GetOptions()
        {
            var section = _configuration.GetSection(typeof(IdServerEmailOptions).Name);
            return section.Get<IdServerEmailOptions>();
        }
    }
}
