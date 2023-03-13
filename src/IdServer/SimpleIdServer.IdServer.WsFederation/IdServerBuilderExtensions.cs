// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.WsFederation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdServerBuilderExtensions
    {
        public static IdServerBuilder AddWsFederation(this IdServerBuilder idServerBuilder, Action<IdServerWsFederationOptions> callback = null)
        {
            if (callback == null) idServerBuilder.Services.Configure<IdServerWsFederationOptions>((o) => { });
            else idServerBuilder.Services.Configure(callback);
            return idServerBuilder;
        }

        public static IdServerBuilder AddWsFederationSigningCredentials(this IdServerBuilder idServerBuilder, string realm = Constants.DefaultRealm)
        {
            var certificate = KeyGenerator.GenerateSelfSignedCertificate();
            var securityKey = new X509SecurityKey(certificate, "wsFedKid");
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
            idServerBuilder.KeyStore.Add(realm, credentials);
            return idServerBuilder;
        }
    }
}
