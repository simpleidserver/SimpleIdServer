// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OpenID;

namespace SimpleIdServer.UI.Authenticate.Email
{
    public class EmailModule : IModule
    {
        public string Name => "EmailModule";

        public void Register(IServiceCollection services)
        {
            SimpleIdServerOpenIDBuilderExtensions.RegisterDependencies(services);
        }
    }
}
