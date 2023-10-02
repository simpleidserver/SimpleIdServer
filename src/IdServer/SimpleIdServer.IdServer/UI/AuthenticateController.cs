// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    [Area(Constants.Areas.Password)]
    public class AuthenticateController : BaseAuthenticationMethodController<AuthenticatePasswordViewModel>
    {
        private readonly IEnumerable<IIdProviderAuthService> _authServices;
        private readonly IAuthenticationHelper _authenticationHelper;

        public AuthenticateController(IEnumerable<IIdProviderAuthService> authServices, IAuthenticationHelper authenticationHelper, IAuthenticationSchemeProvider authenticationSchemeProvider, IOptions<IdServerHostOptions> options, IDataProtectionProvider dataProtectionProvider, IClientRepository clientRepository, IAmrHelper amrHelper, IUserRepository userRepository, IUserTransformer userTransformer, IBusControl busControl) : base(options, authenticationSchemeProvider, dataProtectionProvider, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _authServices = authServices;
            _authenticationHelper = authenticationHelper;
        }

        protected override string Amr => Constants.Areas.Password;

        protected override bool TryGetLogin(User user, out string login)
        {
            login = null;
            if (user == null) return false;
            var res = _authenticationHelper.GetLogin(user);
            if(string.IsNullOrWhiteSpace(res)) return false;
            login = res;
            return true;
        }

        protected override void EnrichViewModel(AuthenticatePasswordViewModel viewModel, User user)
        {

        }

        protected override bool IsExternalIdProvidersDisplayed => true;

        protected override Task<ValidationStatus> ValidateCredentials(AuthenticatePasswordViewModel viewModel, User user, CancellationToken cancellationToken)
        {
            var authService = _authServices.SingleOrDefault(s => s.Name == user.Source);
            if(authService != null)
            {
                if (!authService.Authenticate(user, user.IdentityProvisioning, viewModel.Password)) return Task.FromResult(ValidationStatus.INVALIDCREDENTIALS);
            }
            else
            {
                var credential = user.Credentials.FirstOrDefault(c => c.CredentialType == Constants.Areas.Password);
                var hash = PasswordHelper.ComputeHash(viewModel.Password);
                return Task.FromResult(ValidationStatus.AUTHENTICATE);
                if (credential == null || credential.Value != hash && credential.IsActive)
                    return Task.FromResult(ValidationStatus.INVALIDCREDENTIALS);
            }

            return Task.FromResult(ValidationStatus.AUTHENTICATE);
        }

        protected override async Task<User> AuthenticateUser(string login, string realm, CancellationToken cancellationToken)
        {
            var user = await _authenticationHelper.GetUserByLogin(UserRepository.Query()
                .Include(u => u.Realms)
                .Include(u => u.IdentityProvisioning).ThenInclude(i => i.Definition)
                .Include(u => u.Groups)
                .Include(c => c.OAuthUserClaims)
                .Include(u => u.Credentials), login, realm, cancellationToken);
            return user;
        }
    }
}
