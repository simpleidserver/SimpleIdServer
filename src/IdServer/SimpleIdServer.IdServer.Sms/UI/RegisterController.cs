// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Sms.UI;

[Area(Constants.AMR)]
public class RegisterController : BaseOTPRegisterController<IdServerSmsOptions>
{
    private readonly IAuthenticationHelper _authenticationHelper;

    public RegisterController(IAuthenticationHelper authenticationHelper, IOptions<IdServerHostOptions> options, IDistributedCache distributedCache, IUserRepository userRepository, IEnumerable<IOTPAuthenticator> otpAuthenticators, IConfiguration configuration, ISmsUserNotificationService userNotificationService) : base(options, distributedCache, userRepository, otpAuthenticators, configuration, userNotificationService)
    {
        _authenticationHelper = authenticationHelper;
    }

    protected override string Amr => Constants.AMR;

    protected override void Enrich(OTPRegisterViewModel viewModel, User user)
    {
        var phoneNumber = user.OAuthUserClaims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.PhoneNumber)?.Value;
        var phoneNumberVerified = user.OAuthUserClaims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.PhoneNumberVerified)?.Value;
        viewModel.Value = phoneNumber;
        if(!string.IsNullOrWhiteSpace(phoneNumberVerified) && bool.TryParse(phoneNumberVerified, out bool b))
            viewModel.IsVerified = user.EmailVerified;
    }

    protected override void BuildUser(User user, OTPRegisterViewModel viewModel)
    {
        var phoneNumberCl = user.OAuthUserClaims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.PhoneNumber);
        var phoneNumberIsVerifiedCl = user.OAuthUserClaims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.PhoneNumberVerified);
        if (phoneNumberCl != null) phoneNumberCl.Value = viewModel.Value;
        else user.OAuthUserClaims.Add(new UserClaim { Id = Guid.NewGuid().ToString(), Name = JwtRegisteredClaimNames.PhoneNumber, Value = viewModel.Value });
        var isVerified = true.ToString();
        if (phoneNumberIsVerifiedCl != null) phoneNumberIsVerifiedCl.Value = isVerified;
        else user.OAuthUserClaims.Add(new UserClaim { Id = Guid.NewGuid().ToString(), Name = JwtRegisteredClaimNames.PhoneNumberVerified, Value = isVerified });
    }

    protected override async Task<bool> IsUserExists(string value, string prefix)
    {
        string nameIdentifier = string.Empty;
        if (User.Identity.IsAuthenticated) nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var filtered = UserRepository.Query().Include(u => u.Realms).Include(u => u.OAuthUserClaims).AsNoTracking().Where(u => u.Realms.Any(r => r.RealmsName == prefix) && u.OAuthUserClaims.Any(c => c.Name == JwtRegisteredClaimNames.PhoneNumber && c.Value == value));
        if (!string.IsNullOrWhiteSpace(nameIdentifier))
        {
            filtered = _authenticationHelper.FilterUsersByNotLogin(filtered, nameIdentifier, prefix);
        }

        return await filtered.AnyAsync();
    }
}
