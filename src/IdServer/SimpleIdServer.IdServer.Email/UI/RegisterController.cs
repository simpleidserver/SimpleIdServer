// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Email.UI;

[Area(Constants.AMR)]
public class RegisterController : BaseOTPRegisterController<IdServerEmailOptions>
{
    public RegisterController(IOptions<IdServerHostOptions> options, IDistributedCache distributedCache, IUserRepository userRepository, IEnumerable<IOTPAuthenticator> otpAuthenticators, IConfiguration configuration, IEmailUserNotificationService userNotificationService) : base(options, distributedCache, userRepository, otpAuthenticators, configuration, userNotificationService)
    {
    }

    protected override string Amr => Constants.AMR;

    protected override void Enrich(OTPRegisterViewModel viewModel, User user)
    {
        viewModel.Value = user.Email;
        viewModel.IsVerified = user.EmailVerified;
    }

    protected override void BuildUser(User user, OTPRegisterViewModel viewModel)
    {
        user.Email = viewModel.Value;
        user.EmailVerified = true;
    }

    protected override async Task<bool> IsUserExists(string value, string prefix)
    {
        var result = await UserRepository.Query().Include(u => u.Realms).AsNoTracking().AnyAsync(u => u.Realms.Any(r => r.RealmsName == prefix) && u.Email == value);
        return result;
    }
}
