// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Builders
{
    public class WalletClientBuilder
    {
        private readonly Client _client;

        internal WalletClientBuilder(Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Boolean value specifying whether the Credential Issuer expects presentation of a transaction code along with the Token Request in a Pre-Authorized Code Flow. 
        /// </summary>
        /// <returns></returns>
        public WalletClientBuilder RequireTransactionCode()
        {
            _client.IsTransactionCodeRequired = true;
            return this;
        }

        public Client Build() => _client;
    }
}
