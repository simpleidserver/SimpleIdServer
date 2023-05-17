// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Parsers;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdServerBuilderExtensions
    {
        public static IdServerBuilder AddCredentialIssuer(this IdServerBuilder idServerBuilder, Action<CredentialIssuerOptions> callback = null)
        {
            if (callback == null) idServerBuilder.Services.Configure<CredentialIssuerOptions>(o => { });
            else idServerBuilder.Services.Configure(callback);
            idServerBuilder.Services.AddTransient<ICredentialRequestParser, SignedJWTCredentialRequestParser>();
            return idServerBuilder;
        }
    }
}
