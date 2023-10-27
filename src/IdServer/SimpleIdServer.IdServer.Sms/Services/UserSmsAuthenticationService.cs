// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
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
    private readonly IUserClaimsService _userClaimsService;

    public UserSmsAuthenticationService(IUserClaimsService userClaimsService, IEnumerable<IOTPAuthenticator> otpAuthenticators, IAuthenticationHelper authenticationHelper, IUserRepository userRepository) : base(otpAuthenticators, authenticationHelper, userRepository)
    {
        _userClaimsService = userClaimsService;
    }

    protected override async Task<User> GetUser(string authenticatedUserId, BaseOTPAuthenticateViewModel viewModel, string realm, CancellationToken cancellationToken)
    {
        User authenticatedUser = null;
        if (string.IsNullOrWhiteSpace(authenticatedUserId))
        {
            var userId = await _userClaimsService.GetUserId(realm, JwtRegisteredClaimNames.PhoneNumber, viewModel.Login, cancellationToken);
            if (string.IsNullOrWhiteSpace(userId)) return null;
            authenticatedUser = await UserRepository.Query()
                .Include(u => u.Realms)
                .Include(u => u.IdentityProvisioning).ThenInclude(i => i.Definition)
                .Include(u => u.Groups)
                .Include(u => u.Credentials)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }
        else
            authenticatedUser = await FetchAuthenticatedUser(realm, authenticatedUserId, cancellationToken);

        return authenticatedUser;
    }
}
