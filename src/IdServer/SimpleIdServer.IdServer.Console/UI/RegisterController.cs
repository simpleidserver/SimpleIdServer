// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Console.UI;

[Area(Constants.AMR)]
public class RegisterController : BaseOTPRegisterController<IdServerConsoleOptions>
{
    public RegisterController(
        IOptions<IdServerHostOptions> options, 
        IDistributedCache distributedCache, 
        IUserRepository userRepository, 
        IEnumerable<IOTPAuthenticator> otpAuthenticators, 
        IConfiguration configuration, 
        IUserNotificationService userNotificationService, 
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder) : base(options, distributedCache, userRepository, otpAuthenticators, configuration, userNotificationService, tokenRepository, jwtBuilder)
    {
    }

    protected override string Amr => Constants.AMR;

    protected override void BuildUser(User user, OTPRegisterViewModel viewModel)
    {

    }

    protected override void Enrich(OTPRegisterViewModel viewModel, User user)
    {

    }

    protected override Task<bool> IsUserExists(string value, string prefix)
    {
        return Task.FromResult(false);
    }
}
