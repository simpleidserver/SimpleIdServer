// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Fido.Apis;
using SimpleIdServer.IdServer.Fido.Services;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Fido.UI.Mobile
{
    [Area(Constants.MobileAMR)]
    public class AuthenticateController : BaseAuthenticationMethodController<AuthenticateMobileViewModel>
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly Helpers.IUrlHelper _urlHelper;

        public AuthenticateController(
            IConfiguration configuration, 
            IAuthenticationHelper authenticationHelper,
            IMobileAuthenticationService userAuthenticationService,
            IDistributedCache distributedCache, 
            IOptions<IdServerHostOptions> options, 
            IAuthenticationSchemeProvider authenticationSchemeProvider, 
            IDataProtectionProvider dataProtectionProvider,
            IClientRepository clientRepository, 
            IAmrHelper amrHelper, 
            IUserRepository userRepository,
            IUserTransformer userTransformer, 
            IBusControl busControl,
            Helpers.IUrlHelper urlHelper) : base(options, authenticationSchemeProvider, userAuthenticationService, dataProtectionProvider, authenticationHelper, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _configuration = configuration;
            _authenticationHelper = authenticationHelper;
            _distributedCache= distributedCache;
            _urlHelper = urlHelper;
        }

        protected override string Amr => Constants.MobileAMR;

        protected override bool IsExternalIdProvidersDisplayed => false;

        protected override bool TryGetLogin(AmrAuthInfo amr, out string login)
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
            var options = GetFidoOptions();
            var issuer = _urlHelper.GetAbsoluteUriWithVirtualPath(Request);
            viewModel.BeginLoginUrl = $"{issuer}/{viewModel.Realm}/{Constants.EndPoints.BeginQRCodeLogin}";
            viewModel.LoginStatusUrl = $"{issuer}/{viewModel.Realm}/{Constants.EndPoints.LoginStatus}";
            viewModel.IsDeveloperModeEnabled = options.IsDeveloperModeEnabled;
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

        private FidoOptions GetFidoOptions()
        {
            var section = _configuration.GetSection(typeof(FidoOptions).Name);
            return section.Get<FidoOptions>();
        }
    }
}
