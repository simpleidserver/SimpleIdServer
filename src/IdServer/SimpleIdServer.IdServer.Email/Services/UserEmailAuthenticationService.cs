// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Email.Services;

public interface IUserEmailAuthenticationService : IUserAuthenticationService
{

}

public class UserEmailAuthenticationService : OTPAuthenticationService, IUserEmailAuthenticationService
{
    public UserEmailAuthenticationService(IEnumerable<IOTPAuthenticator> otpAuthenticators, IAuthenticationHelper authenticationHelper, IUserRepository userRepository) : base(otpAuthenticators, authenticationHelper, userRepository)
    {
    }

    protected override async Task<User> GetUser(string authenticatedUserId, BaseOTPAuthenticateViewModel viewModel, string realm, CancellationToken cancellationToken)
    {
        User authenticatedUser = null;
        if (string.IsNullOrWhiteSpace(authenticatedUserId))
            authenticatedUser = await UserRepository.Query()
                .Include(u => u.Realms)
                .Include(u => u.IdentityProvisioning).ThenInclude(i => i.Definition)
                .Include(u => u.Groups)
                .Include(u => u.Credentials)
                .FirstOrDefaultAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.Email == viewModel.Login, cancellationToken);
        else
            authenticatedUser = await FetchAuthenticatedUser(realm, authenticatedUserId, cancellationToken);

        return authenticatedUser;
    }
}
