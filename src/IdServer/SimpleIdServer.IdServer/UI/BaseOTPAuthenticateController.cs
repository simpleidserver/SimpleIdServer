// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
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
            IUserTransformer userTransformer,
            IBusControl busControl,
            IUserClaimsService userClaimsService) : base(options, authenticationSchemeProvider, userAuthenticationService, userClaimsService, dataProtectionProvider, authenticationHelper, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _notificationServices = notificationServices;
            _otpAuthenticators = otpAuthenticators;
        }

        protected abstract string FormattedMessage { get; }

        protected override async Task<UserAuthenticationResult> CustomAuthenticate(string prefix, string authenticatedUserId, T viewModel, CancellationToken cancellationToken)
        {
            var authenticatedUser = await UserRepository.Query().Include(u => u.Realms)
                .Include(u => u.Credentials)
                .FirstOrDefaultAsync(u => u.Realms.Any(r => r.RealmsName == prefix) && u.Id == authenticatedUserId, cancellationToken);
            if (authenticatedUser == null)
            {
                ModelState.AddModelError("unknown_user", "unknown_user");
                return UserAuthenticationResult.Error(View(viewModel));
            }

            var activeOtp = authenticatedUser.ActiveOTP;
            var otpAuthenticator = _otpAuthenticators.Single(a => a.Alg == activeOtp.OTPAlg);
            switch(viewModel.Action)
            {
                case "SENDCONFIRMATIONCODE":
                    var notificationService = _notificationServices.First(n => n.Name == Amr);
                    var otpCode = otpAuthenticator.GenerateOtp(activeOtp);
                    await notificationService.Send(string.Format(FormattedMessage, otpCode), authenticatedUser);
                    SetSuccessMessage("confirmationcode_sent");
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
