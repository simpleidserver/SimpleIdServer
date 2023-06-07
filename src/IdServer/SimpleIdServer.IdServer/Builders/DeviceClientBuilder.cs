// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Builders
{
    public class DeviceClientBuilder
    {
        private readonly Client _client;

        internal DeviceClientBuilder(Client client)
        {
            _client = client;
        }

        #region Scopes

        public DeviceClientBuilder AddScope(params Scope[] scopes)
        {
            foreach (var scope in scopes) _client.Scopes.Add(scope);
            return this;
        }

        #endregion

        public Client Build() => _client;
    }
}
