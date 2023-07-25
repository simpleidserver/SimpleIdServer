// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
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
    public abstract class BaseOTPAuthenticateController<T> : BaseAuthenticationMethodController<T> where T : BaseOTPAuthenticateViewModel
    {
        private readonly IEnumerable<IUserNotificationService> _notificationServices;
        private readonly IEnumerable<IOTPAuthenticator> _otpAuthenticators;

        protected BaseOTPAuthenticateController(IEnumerable<IUserNotificationService> notificationServices, IEnumerable<IOTPAuthenticator> otpAuthenticators, IOptions<IdServerHostOptions> options, IAuthenticationSchemeProvider authenticationSchemeProvider, IDataProtectionProvider dataProtectionProvider, IClientRepository clientRepository, IAmrHelper amrHelper, IUserRepository userRepository, IUserTransformer userTransformer, IBusControl busControl) : base(options, authenticationSchemeProvider, dataProtectionProvider, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _notificationServices = notificationServices;
            _otpAuthenticators = otpAuthenticators;
        }

        protected abstract string FormattedMessage { get; }

        protected override async Task<ValidationStatus> ValidateCredentials(T viewModel, User user, CancellationToken cancellationToken)
        {
            var activeOtp = user.ActiveOTP;
            var otpAuthenticator = _otpAuthenticators.Single(a => a.Alg == activeOtp.OTPAlg);
            switch (viewModel.Action)
            {
                case "SENDCONFIRMATIONCODE":
                    var notificationService = _notificationServices.First(n => n.Name == Amr);
                    var otpCode = otpAuthenticator.GenerateOtp(activeOtp);
                    // await notificationService.Send(string.Format(FormattedMessage, otpCode), user);
                    SetSuccessMessage("confirmationcode_sent");
                    return ValidationStatus.NOCONTENT;
                default:
                    viewModel.CheckConfirmationCode(ModelState);
                    if (!ModelState.IsValid) return ValidationStatus.NOCONTENT;
                    if (!otpAuthenticator.Verify(viewModel.OTPCode.Value, activeOtp)) return ValidationStatus.INVALIDCREDENTIALS;
                    return ValidationStatus.AUTHENTICATE;
            }
        }
    }
}
