// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Email.UI;

[Area(Constants.AMR)]
public class RegisterController : BaseOTPRegisterController<IdServerEmailOptions>
{
    private readonly IAuthenticationHelper _authenticationHelper;

    public RegisterController(IAuthenticationHelper authenticationHelper, IOptions<IdServerHostOptions> options, IDistributedCache distributedCache, IUserRepository userRepository, IEnumerable<IOTPAuthenticator> otpAuthenticators, IConfiguration configuration, IEmailUserNotificationService userNotificationService, IUserClaimsService userClaimsService) : base(options, distributedCache, userRepository, userClaimsService, otpAuthenticators, configuration, userNotificationService)
    {
        _authenticationHelper = authenticationHelper;
    }

    protected override string Amr => Constants.AMR;

    protected override void Enrich(OTPRegisterViewModel viewModel, User user, ICollection<UserClaim> userClaims)
    {
        viewModel.Value = user.Email;
        viewModel.IsVerified = user.EmailVerified;
    }

    protected override void BuildUser(User user, ICollection<UserClaim> userClaims, OTPRegisterViewModel viewModel)
    {
        user.Email = viewModel.Value;
        user.EmailVerified = true;
    }

    protected override async Task<bool> IsUserExists(string value, string prefix)
    {
        string nameIdentifier = string.Empty;
        if(User.Identity.IsAuthenticated) nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var filtered = UserRepository.Query().Include(u => u.Realms).AsNoTracking().Where(u => u.Realms.Any(r => r.RealmsName == prefix) && u.Email == value);
        if(!string.IsNullOrWhiteSpace(nameIdentifier))
        {
            filtered = _authenticationHelper.FilterUsersByNotLogin(filtered, nameIdentifier, prefix);
        }

        return await filtered.AnyAsync();
    }
}
