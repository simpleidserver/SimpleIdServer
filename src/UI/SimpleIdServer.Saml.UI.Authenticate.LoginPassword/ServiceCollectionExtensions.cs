// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Saml.Idp;
using SimpleIdServer.Saml.UI.Authenticate.LoginPassword;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSamlLoginPawdAuth(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAuthenticator, LoginPwdAuthenticator>();
            return serviceCollection;
        }
    }
}
