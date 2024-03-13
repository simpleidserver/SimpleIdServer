// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using SimpleIdServer.IdServer.VerifiablePresentation.UI.ViewModels;

namespace SimpleIdServer.IdServer.VerifiablePresentation.UI
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseAuthenticationMethodController<VpAuthenticateViewModel>
    {
        private readonly IPresentationDefinitionStore _presentationDefinitionStore;

        public AuthenticateController(
            IOptions<IdServerHostOptions> options,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IUserAuthenticationService userAuthenticationService,
            IDataProtectionProvider dataProtectionProvider,
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder,
            IAuthenticationHelper authenticationHelper,
            IClientRepository clientRepository,
            IAmrHelper amrHelper,
            IUserRepository userRepository,
            IUserSessionResitory userSessionRepository,
            IUserTransformer userTransformer,
            IBusControl busControl,
            IPresentationDefinitionStore presentationDefinitionStore) : base(options, authenticationSchemeProvider, userAuthenticationService, dataProtectionProvider, tokenRepository, jwtBuilder, authenticationHelper, clientRepository, amrHelper, userRepository, userSessionRepository, userTransformer, busControl)
        {
            _presentationDefinitionStore = presentationDefinitionStore;
        }

        protected override string Amr => Constants.AMR;

        protected override bool IsExternalIdProvidersDisplayed => false;

        protected override Task<UserAuthenticationResult> CustomAuthenticate(string prefix, string authenticatedUserId, VpAuthenticateViewModel viewModel, CancellationToken cancellationToken)
        {
            return Task.FromResult(UserAuthenticationResult.Ok());
        }

        protected override void EnrichViewModel(VpAuthenticateViewModel viewModel)
        {
            var presentationDefinitions = _presentationDefinitionStore.Query().ToList();
            viewModel.PresentationDefinitions = presentationDefinitions.Select(d => new PresentationDefinitionViewModel
            {
                Name = d.Name,
                PublicId = d.PublicId,
                Purpose = d.Purpose
            }).ToList();
        }

        protected override bool TryGetLogin(AmrAuthInfo amrInfo, out string login)
        {
            login = null;
            if (amrInfo == null || string.IsNullOrWhiteSpace(amrInfo.Login)) return false;
            login = amrInfo.Login;
            return true;
        }
    }
}