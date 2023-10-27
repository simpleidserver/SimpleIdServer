// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI.Services;

public abstract class OTPAuthenticationService : GenericAuthenticationService<BaseOTPAuthenticateViewModel>
{
    private readonly IEnumerable<IOTPAuthenticator> _otpAuthenticators;

    public OTPAuthenticationService(IEnumerable<IOTPAuthenticator> otpAuthenticators, IAuthenticationHelper authenticationHelper, IUserRepository userRepository) : base(authenticationHelper, userRepository)
    {
        _otpAuthenticators = otpAuthenticators;
    }

    protected override async Task<CredentialsValidationResult> Validate(string realm, string authenticatedUserId, BaseOTPAuthenticateViewModel viewModel, CancellationToken cancellationToken)
    {
        var authenticatedUser = await GetUser(authenticatedUserId, viewModel, realm, cancellationToken);
        if (authenticatedUser == null) return CredentialsValidationResult.Error(ValidationStatus.UNKNOWN_USER);
        return await Validate(realm, authenticatedUser, null, viewModel, cancellationToken);
    }

    protected override Task<CredentialsValidationResult> Validate(string realm, User authenticatedUser, ICollection<Claim> claims, BaseOTPAuthenticateViewModel viewModel, CancellationToken cancellationToken)
    {
        if (authenticatedUser.Email != viewModel.Login) return Task.FromResult(CredentialsValidationResult.Error("bad_email", "bad_email"));
        if (authenticatedUser.ActiveOTP == null) return Task.FromResult(CredentialsValidationResult.Error("no_active_otp", "no_active_otp"));
        var activeOtp = authenticatedUser.ActiveOTP;
        var otpAuthenticator = _otpAuthenticators.Single(a => a.Alg == activeOtp.OTPAlg);
        if (!otpAuthenticator.Verify(viewModel.OTPCode.Value, activeOtp)) return Task.FromResult(CredentialsValidationResult.Error(ValidationStatus.INVALIDCREDENTIALS));
        return Task.FromResult(CredentialsValidationResult.Ok(authenticatedUser));
    }

    protected virtual async Task<User> GetUser(string authenticatedUserId, BaseOTPAuthenticateViewModel viewModel, string realm, CancellationToken cancellationToken)
    {
        User authenticatedUser = null;
        if (string.IsNullOrWhiteSpace(authenticatedUserId))
            authenticatedUser = await AuthenticateUser(viewModel.Login, realm, cancellationToken);
        else
            authenticatedUser = await FetchAuthenticatedUser(realm, authenticatedUserId, cancellationToken);
        return authenticatedUser;
    }
}
