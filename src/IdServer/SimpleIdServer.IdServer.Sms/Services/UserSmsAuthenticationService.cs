// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Sms.Services;

public interface IUserSmsAuthenticationService : IUserAuthenticationService
{

}

public class UserSmsAuthenticationService : OTPAuthenticationService, IUserSmsAuthenticationService
{
    public UserSmsAuthenticationService(IEnumerable<IOTPAuthenticator> otpAuthenticators, IAuthenticationHelper authenticationHelper, IUserRepository userRepository) : base(otpAuthenticators, authenticationHelper, userRepository)
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
                .Include(c => c.OAuthUserClaims)
                .Include(u => u.Credentials)
                .FirstOrDefaultAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.OAuthUserClaims.Any(c => c.Name == JwtRegisteredClaimNames.PhoneNumber && c.Value == viewModel.Login), cancellationToken);
        else
            authenticatedUser = await FetchAuthenticatedUser(realm, authenticatedUserId, cancellationToken);

        return authenticatedUser;
    }
}
