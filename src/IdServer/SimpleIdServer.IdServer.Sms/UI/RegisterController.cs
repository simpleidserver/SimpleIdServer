// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
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

    public RegisterController(IAuthenticationHelper authenticationHelper, IUserClaimsService userClaimsService, IOptions<IdServerHostOptions> options, IDistributedCache distributedCache, IUserRepository userRepository, IEnumerable<IOTPAuthenticator> otpAuthenticators, IConfiguration configuration, ISmsUserNotificationService userNotificationService) : base(options, distributedCache, userRepository, userClaimsService, otpAuthenticators, configuration, userNotificationService)
    {
        _authenticationHelper = authenticationHelper;
    }

    protected override string Amr => Constants.AMR;

    protected override void Enrich(OTPRegisterViewModel viewModel, User user, ICollection<UserClaim> claims)
    {
        var phoneNumber = claims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.PhoneNumber)?.Value;
        var phoneNumberVerified = claims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.PhoneNumberVerified)?.Value;
        viewModel.Value = phoneNumber;
        if(!string.IsNullOrWhiteSpace(phoneNumberVerified) && bool.TryParse(phoneNumberVerified, out bool b))
            viewModel.IsVerified = user.EmailVerified;
    }

    protected override void BuildUser(User user, ICollection<UserClaim> claims, OTPRegisterViewModel viewModel)
    {
        var phoneNumberCl = claims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.PhoneNumber);
        var phoneNumberIsVerifiedCl = claims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.PhoneNumberVerified);
        if (phoneNumberCl != null) phoneNumberCl.Value = viewModel.Value;
        else claims.Add(new UserClaim { Id = Guid.NewGuid().ToString(), Name = JwtRegisteredClaimNames.PhoneNumber, Value = viewModel.Value });
        var isVerified = true.ToString();
        if (phoneNumberIsVerifiedCl != null) phoneNumberIsVerifiedCl.Value = isVerified;
        else claims.Add(new UserClaim { Id = Guid.NewGuid().ToString(), Name = JwtRegisteredClaimNames.PhoneNumberVerified, Value = isVerified });
    }

    protected override async Task<bool> IsUserExists(string value, string prefix)
    {
        string nameIdentifier = string.Empty;
        if (User.Identity.IsAuthenticated) nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var existingUserId = await UserClaimsService.GetUserId(prefix, JwtRegisteredClaimNames.PhoneNumber, value, CancellationToken.None);
        if (string.IsNullOrWhiteSpace(existingUserId)) return false;
        var existingUsers = await _authenticationHelper.FilterUsersByLogin(UserRepository.Query(), nameIdentifier, prefix).ToListAsync();
        return !existingUsers.Any(u => u.Id == existingUserId);
    }
}
