// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Otp;
using SimpleIdServer.IdServer.Otp.Services;
using SimpleIdServer.IdServer.UI.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    /// <summary>
    /// Add otp authentication method.
    /// The amr is otp.
    /// </summary>
    /// <param name="idServerBuilder"></param>
    /// <returns></returns>
    public static IdServerBuilder AddOtpAuthentication(this IdServerBuilder idServerBuilder)
    {
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, OtpAuthenticationMethodService>();
        idServerBuilder.Services.AddTransient<IOtpAuthenticationService, OtpAuthenticationService>();
        idServerBuilder.Services.AddTransient<IUserAuthenticationService, OtpAuthenticationService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, OtpAuthWorkflowLayout>();
        return idServerBuilder;
    }
}
