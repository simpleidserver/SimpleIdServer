// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.WsFederation.Extensions;

namespace SimpleIdServer.IdServer.Builders
{
    public static class TraditionalWebsiteClientBuilderExtensions
    {
        /// <summary>
        /// Enable ws-federation protocol.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static TraditionalWebsiteClientBuilder EnableWsFederationProtocol(this TraditionalWebsiteClientBuilder builder)
        {
            builder.Client.SetWsFederationEnabled(true);
            return builder;
        }
    }
}
