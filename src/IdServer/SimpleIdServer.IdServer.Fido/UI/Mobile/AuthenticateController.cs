// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
using System.Text.Json;

namespace SimpleIdServer.IdServer.Fido.UI.Mobile
{
    [Area(Constants.MobileAMR)]
    public class AuthenticateController : BaseAuthenticationMethodController<AuthenticateMobileViewModel>
    {
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IDistributedCache _distributedCache;

        public AuthenticateController(IAuthenticationHelper authenticationHelper, IDistributedCache distributedCache, IOptions<IdServerHostOptions> options, IAuthenticationSchemeProvider authenticationSchemeProvider, IDataProtectionProvider dataProtectionProvider, IClientRepository clientRepository, IAmrHelper amrHelper, IUserRepository userRepository, IUserTransformer userTransformer, IBusControl busControl) : base(options, authenticationSchemeProvider, dataProtectionProvider, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _authenticationHelper = authenticationHelper;
            _distributedCache= distributedCache;
        }

        protected override string Amr => Constants.MobileAMR;

        protected override bool IsExternalIdProvidersDisplayed => false;

        protected override bool TryGetLogin(User user, out string login)
        {
            login = null;
            if (user == null) return false;
            var res = _authenticationHelper.GetLogin(user);
            if (string.IsNullOrWhiteSpace(res)) return false;
            login = res;
            return true;
        }

        protected override void EnrichViewModel(AuthenticateMobileViewModel viewModel, User user)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            viewModel.BeginLoginUrl = $"{issuer}/{viewModel.Realm}/{Constants.EndPoints.BeginQRCodeLogin}";
            viewModel.LoginStatusUrl = $"{issuer}/{viewModel.Realm}/{Constants.EndPoints.LoginStatus}";
        }

        protected override async Task<ValidationStatus> ValidateCredentials(AuthenticateMobileViewModel viewModel, User user, CancellationToken cancellationToken)
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

        protected override async Task<User> AuthenticateUser(string login, string realm, CancellationToken cancellationToken)
        {
            var user = await _authenticationHelper.GetUserByLogin(UserRepository.Query()
                .Include(u => u.Realms)
                .Include(u => u.IdentityProvisioning).ThenInclude(i => i.Properties)
                .Include(u => u.Groups)
                .Include(c => c.OAuthUserClaims)
                .Include(u => u.Credentials), login, realm, cancellationToken);
            return user;
        }
    }
}
