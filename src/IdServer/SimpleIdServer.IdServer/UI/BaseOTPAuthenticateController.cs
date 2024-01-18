// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
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
    public abstract class BaseOTPAuthenticateController<T> : BaseAuthenticationMethodController<T> where T : BaseOTPAuthenticateViewModel
    {
        private readonly IEnumerable<IUserNotificationService> _notificationServices;
        private readonly IEnumerable<IOTPAuthenticator> _otpAuthenticators;

        protected BaseOTPAuthenticateController(
            IEnumerable<IUserNotificationService> notificationServices,
            IEnumerable<IOTPAuthenticator> otpAuthenticators,
            IUserAuthenticationService userAuthenticationService,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IOptions<IdServerHostOptions> options,
            IDataProtectionProvider dataProtectionProvider,
            IAuthenticationHelper authenticationHelper,
            IClientRepository clientRepository,
            IAmrHelper amrHelper,
            IUserRepository userRepository,
            IUserSessionResitory userSessionRepository,
            IUserTransformer userTransformer,
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder,
            IBusControl busControl) : base(options, authenticationSchemeProvider, userAuthenticationService, dataProtectionProvider, tokenRepository, jwtBuilder, authenticationHelper, clientRepository, amrHelper, userRepository, userSessionRepository, userTransformer, busControl)
        {
            _notificationServices = notificationServices;
            _otpAuthenticators = otpAuthenticators;
        }

        protected abstract string FormattedMessage { get; }

        protected override async Task<UserAuthenticationResult> CustomAuthenticate(string prefix, string authenticatedUserId, T viewModel, CancellationToken cancellationToken)
        {
            var authenticatedUser = await UserAuthenticationService.GetUser(authenticatedUserId, viewModel, prefix, cancellationToken);
            if (authenticatedUser == null)
            {
                ModelState.AddModelError("unknown_user", "unknown_user");
                return UserAuthenticationResult.Error(View(viewModel));
            }

            var activeOtp = authenticatedUser.ActiveOTP;
            if(activeOtp == null)
            {
                ModelState.AddModelError("no_active_otp", "no_active_otp");
                return UserAuthenticationResult.Error(View(viewModel));
            }

            var otpAuthenticator = _otpAuthenticators.Single(a => a.Alg == activeOtp.OTPAlg);
            switch(viewModel.Action)
            {
                case "SENDCONFIRMATIONCODE":
                    var notificationService = _notificationServices.First(n => n.Name == Amr);
                    var otpCode = otpAuthenticator.GenerateOtp(activeOtp);
                    await notificationService.Send("One Time Password", string.Format(FormattedMessage, otpCode), new Dictionary<string, string>(), authenticatedUser);
                    SetSuccessMessage("confirmationcode_sent");
                    if(activeOtp.OTPAlg == Domains.OTPAlgs.TOTP) viewModel.TOTPStep = activeOtp.TOTPStep;
                    return UserAuthenticationResult.Error(View(viewModel));
                default:
                    viewModel.CheckConfirmationCode(ModelState);
                    if (!ModelState.IsValid) return UserAuthenticationResult.Error(View(viewModel));
                    break;
            }

            return UserAuthenticationResult.Ok(authenticatedUser);
        }

        protected override void EnrichViewModel(T viewModel)
        {

        }
    }
}
