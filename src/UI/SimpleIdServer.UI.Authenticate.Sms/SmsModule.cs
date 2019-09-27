// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OpenID;

namespace SimpleIdServer.UI.Authenticate.Sms
{
    public class SmsModule : IModule
    {
        public string Name => "SmsModule";

        public void Register(IServiceCollection services)
        {
            SimpleIdServerOpenIDBuilderExtensions.RegisterDependencies(services);
        }
    }
}
