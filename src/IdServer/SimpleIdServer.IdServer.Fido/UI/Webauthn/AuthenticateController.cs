// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Fido.Apis;
using SimpleIdServer.IdServer.Fido.Services;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
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
        private readonly IDistributedCache _distributedCache;

        public AuthenticateController(IAuthenticationHelper authenticationHelper,
            IDistributedCache distributedCache,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IWebauthnAuthenticationService userAuthenticationService,
            IOptions<IdServerHostOptions> options,
            IDataProtectionProvider dataProtectionProvider,
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder,
            IClientRepository clientRepository,
            IAmrHelper amrHelper,
            IUserRepository userRepository,
            IUserSessionResitory userSessionRepository,
            IUserTransformer userTransformer,
            IBusControl busControl,
            IAntiforgery antiforgery) : base(options, authenticationSchemeProvider, userAuthenticationService, dataProtectionProvider, tokenRepository, jwtBuilder, authenticationHelper, clientRepository, amrHelper, userRepository, userSessionRepository, userTransformer, busControl, antiforgery)
        {
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

        protected override Task<UserAuthenticationResult> CustomAuthenticate(string prefix, string authenticatedUserId, AuthenticateWebauthnViewModel viewModel, CancellationToken cancellationToken)
        {
            return Task.FromResult(UserAuthenticationResult.Ok());
        }


        protected override void EnrichViewModel(AuthenticateWebauthnViewModel viewModel)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
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
