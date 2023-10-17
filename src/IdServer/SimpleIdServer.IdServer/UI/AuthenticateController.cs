// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    [Area(Constants.Areas.Password)]
    public class AuthenticateController : BaseAuthenticationMethodController<AuthenticatePasswordViewModel>
    {
        public AuthenticateController(IPasswordAuthenticationService userAuthenticationService, IAuthenticationSchemeProvider authenticationSchemeProvider, IOptions<IdServerHostOptions> options, IDataProtectionProvider dataProtectionProvider, IAuthenticationHelper authenticationHelper, IClientRepository clientRepository, IAmrHelper amrHelper, IUserRepository userRepository, IUserTransformer userTransformer, IBusControl busControl) : base(options, authenticationSchemeProvider, userAuthenticationService, dataProtectionProvider, authenticationHelper, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
        }

        protected override string Amr => Constants.Areas.Password;

        protected override bool IsExternalIdProvidersDisplayed => true;

        protected override Task<UserAuthenticationResult> CustomAuthenticate(string prefix, string authenticatedUserId, AuthenticatePasswordViewModel viewModel, CancellationToken cancellationToken)
        {
            return Task.FromResult(UserAuthenticationResult.Ok());
        }

        protected override bool TryGetLogin(AmrAuthInfo amrInfo, out string login)
        {
            login = amrInfo.Login;
            return true;
        }

        protected override void EnrichViewModel(AuthenticatePasswordViewModel viewModel)
        {

        }
    }
}
