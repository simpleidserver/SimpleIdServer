// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Sms.UI;

[Area(Constants.AMR)]
public class RegisterController : BaseOTPRegisterController<IdServerSmsOptions>
{
    public RegisterController(IOptions<IdServerHostOptions> options, IDistributedCache distributedCache, IUserRepository userRepository, IEnumerable<IOTPAuthenticator> otpAuthenticators, IConfiguration configuration, ISmsUserNotificationService userNotificationService) : base(options, distributedCache, userRepository, otpAuthenticators, configuration, userNotificationService)
    {
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
        if (phoneNumberIsVerifiedCl != null) phoneNumberIsVerifiedCl.Value = viewModel.IsVerified.ToString();
        else user.OAuthUserClaims.Add(new UserClaim { Id = Guid.NewGuid().ToString(), Name = JwtRegisteredClaimNames.PhoneNumberVerified, Value = viewModel.IsVerified.ToString() });
    }

    protected override async Task<bool> IsUserExists(string value, string prefix)
    {
        var result = await UserRepository.Query().Include(u => u.Realms).Include(u => u.OAuthUserClaims).AsNoTracking().AnyAsync(u => u.Realms.Any(r => r.RealmsName == prefix) && u.OAuthUserClaims.Any(c => c.Name == JwtRegisteredClaimNames.PhoneNumber && c.Value == value));
        return result;
    }
}
