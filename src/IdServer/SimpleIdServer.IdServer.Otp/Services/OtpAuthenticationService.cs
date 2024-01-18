// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Otp.Services;

public interface IOtpAuthenticationService : IUserAuthenticationService
{

}

public class OtpAuthenticationService : BaseOTPAuthenticationService, IOtpAuthenticationService
{
    public OtpAuthenticationService(
        IEnumerable<IOTPAuthenticator> otpAuthenticators, 
        IAuthenticationHelper authenticationHelper, 
        IUserRepository userRepository) : base(otpAuthenticators, authenticationHelper, userRepository)
    {
    }

    public override string Amr => Constants.Amr;

    protected override async Task<User> GetUser(string authenticatedUserId, BaseOTPAuthenticateViewModel viewModel, string realm, CancellationToken cancellationToken)
    {
        User authenticatedUser = null;
        if (string.IsNullOrWhiteSpace(authenticatedUserId))
            authenticatedUser = await AuthenticateUser(viewModel.Login, realm, cancellationToken);
        else
            authenticatedUser = await FetchAuthenticatedUser(realm, authenticatedUserId, cancellationToken);

        return authenticatedUser;
    }
}
