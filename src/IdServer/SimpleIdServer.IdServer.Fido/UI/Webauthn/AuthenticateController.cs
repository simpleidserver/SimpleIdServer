// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Fido.Apis;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Fido.UI.Webauthn
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseAuthenticationMethodController<AuthenticateWebauthnViewModel>
    {
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IDistributedCache _distributedCache;

        public AuthenticateController(IAuthenticationHelper authenticationHelper,
            IDistributedCache distributedCache,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IUserAuthenticationService userAuthenticationService,
            IOptions<IdServerHostOptions> options,
            IDataProtectionProvider dataProtectionProvider,
            IClientRepository clientRepository,
            IAmrHelper amrHelper,
            IUserRepository userRepository,
            IUserTransformer userTransformer,
            IBusControl busControl) : base(options, authenticationSchemeProvider, userAuthenticationService, dataProtectionProvider, authenticationHelper, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _authenticationHelper = authenticationHelper;
            _distributedCache = distributedCache;
        }

        protected override string Amr => Constants.AMR;

        protected override bool IsExternalIdProvidersDisplayed => false;

        protected override bool TryGetLogin(AmrAuthInfo amr, out string login)
        {
            login = null;
            if (amr == null || string.IsNullOrWhiteSpace(amr.Login)) return false;
            login = amr.Login;
            return true;
        }

        protected void EnrichViewModel(AuthenticateWebauthnViewModel viewModel, User user)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            if (user != null && !user.GetStoredFidoCredentials().Any()) viewModel.IsFidoCredentialsMissing = true;
            viewModel.BeginLoginUrl = $"{issuer}/{viewModel.Realm}/{Constants.EndPoints.BeginLogin}";
            viewModel.EndLoginUrl = $"{issuer}/{viewModel.Realm}/{Constants.EndPoints.EndLogin}";
        }

        protected async Task<ValidationStatus> ValidateCredentials(AuthenticateWebauthnViewModel viewModel, User user, CancellationToken cancellationToken)
        {
            var session = await _distributedCache.GetStringAsync(viewModel.SessionId, cancellationToken);
            if (string.IsNullOrWhiteSpace(session))
            {
                ModelState.AddModelError("unknown_session", "unknown_session");
                return ValidationStatus.NOCONTENT;
            }

            var sessionRecord = JsonSerializer.Deserialize<AuthenticationSessionRecord>(session);
            if(!sessionRecord.IsValidated)
            {
                ModelState.AddModelError("session_not_validated", "session_not_validated");
                return ValidationStatus.NOCONTENT;
            }

            return ValidationStatus.AUTHENTICATE;
        }
    }
}
