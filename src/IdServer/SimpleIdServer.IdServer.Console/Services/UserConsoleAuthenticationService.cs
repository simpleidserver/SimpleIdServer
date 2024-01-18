// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Console.Services;

public interface IUserConsoleAuthenticationService : IUserAuthenticationService
{

}

public class UserConsoleAuthenticationService : BaseOTPAuthenticationService, IUserConsoleAuthenticationService
{
    public UserConsoleAuthenticationService(
        IEnumerable<IOTPAuthenticator> otpAuthenticators, 
        IAuthenticationHelper authenticationHelper, 
        IUserRepository userRepository) : base(otpAuthenticators, authenticationHelper, userRepository)
    {
    }

    public override string Amr => Constants.AMR;

    protected override async Task<User> GetUser(string authenticatedUserId, BaseOTPAuthenticateViewModel viewModel, string realm, CancellationToken cancellationToken)
    {
        User authenticatedUser = null;
        if (string.IsNullOrWhiteSpace(authenticatedUserId))
            authenticatedUser = await AuthenticationHelper.GetUserByLogin(viewModel.Login, realm, cancellationToken);
        else
            authenticatedUser = await FetchAuthenticatedUser(realm, authenticatedUserId, cancellationToken);

        return authenticatedUser;
    }
}
